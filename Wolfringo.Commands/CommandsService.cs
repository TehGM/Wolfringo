using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TehGM.Wolfringo.Commands.Initialization;
using TehGM.Wolfringo.Messages;
using TehGM.Wolfringo.Utilities.Internal;

namespace TehGM.Wolfringo.Commands
{
    public class CommandsService : IDisposable
    {
        private readonly IWolfClient _client;
        private readonly CommandsOptions _options;
        private readonly IServiceProvider _services;
        private readonly ICommandHandlerProvider _handlerProvider;
        private readonly ICommandInitializerMap _initializers;
        private readonly ILogger _log;

        public CancellationToken CancellationToken { get; set; }

        private List<(CommandAttributeBase, MethodInfo)> _commands;
        private readonly SemaphoreSlim _lock;

        public CommandsService(IWolfClient client, CommandsOptions options, IServiceProvider services = null, ICommandHandlerProvider handlerProvider = null, ICommandInitializerMap initializers = null,
            ILogger log = null, CancellationToken cancellationToken = default)
        {
            // init required
            this._client = client ?? throw new ArgumentNullException(nameof(client));
            this._options = options ?? throw new ArgumentNullException(nameof(options));

            // init optionals
            this._log = log;
            this._services = services ?? this.CreateDefaultServiceProvider();
            this._handlerProvider = handlerProvider ?? new CommandHandlerProvider(this._services);
            this._initializers = initializers ?? new DefaultCommandInitializerMap();
            this.CancellationToken = cancellationToken;

            // init private
            this._commands = new List<(CommandAttributeBase, MethodInfo)>();
            this._lock = new SemaphoreSlim(1, 1);

            // register event handlers
            this._client.AddMessageListener<ChatMessage>(OnMessageReceived);
        }

        private IServiceProvider CreateDefaultServiceProvider()
        {
            IDictionary<Type, object> servicesMap = new Dictionary<Type, object>
                {
                    { typeof(IWolfClient), this._client },
                    { this._client.GetType(), this._client },
                    { typeof(CommandsOptions), this._options }
                };
            if (this._log != null)
            {
                servicesMap.Add(typeof(ILogger), this._log);
                servicesMap.Add(this._log.GetType(), this._log);
            }
            return new SimpleServiceProvider(servicesMap);
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            using (CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, this.CancellationToken))
            {
                await this._lock.WaitAsync(cts.Token).ConfigureAwait(false);
                try
                {
                    this._log?.LogDebug("Initializing commands");

                    this._commands.Clear();
                    foreach (Assembly asm in _options.Assemblies)
                        this.AddAssembly(asm);
                    foreach (Type t in _options.Classes)
                        this.AddType(t.GetTypeInfo());

                    this._log?.LogDebug("{Count} commands loaded", _commands.Count);
                }
                finally
                {
                    this._lock.Release();
                }
            }
        }

        private void AddAssembly(Assembly assembly)
        {
            _log?.LogTrace("Loading assembly {Name}", assembly.FullName);
            IEnumerable<TypeInfo> types = assembly.DefinedTypes.Where(t => !t.IsAbstract && !t.ContainsGenericParameters
                && !Attribute.IsDefined(t, typeof(CompilerGeneratedAttribute)) && Attribute.IsDefined(t, typeof(CommandHandlerAttribute), true));
            if (!types.Any())
            {
                _log?.LogWarning("Cannot initialize commands from assembly {Name} - no non-static non-abstract non-generic classes with {Attribute}", assembly.FullName, nameof(CommandHandlerAttribute));
                return;
            }
            foreach (TypeInfo type in types)
                AddType(type);
        }

        private void AddType(TypeInfo type)
        {
            _log?.LogTrace("Loading handler {Handler}", type.FullName);
            IEnumerable<MethodInfo> methods = type.DeclaredMethods.Where(m => !m.IsStatic && !Attribute.IsDefined(m, typeof(CompilerGeneratedAttribute)) && Attribute.IsDefined(m, typeof(CommandAttributeBase), true));
            if (!methods.Any())
            {
                _log?.LogWarning("Cannot initialize commands from type {Handler} - no method with {Attribute}", type.FullName, nameof(CommandAttributeBase));
                return;
            }
            foreach (MethodInfo method in methods)
                AddMethod(method);
        }

        private void AddMethod(MethodInfo method)
        {
            _log?.LogTrace("Loading command {Name}", method.Name);
            Type handlerType = method.DeclaringType;
            IEnumerable<CommandAttributeBase> attributes = method.GetCustomAttributes<CommandAttributeBase>(true);
            if (!attributes.Any())
            {
                _log?.LogWarning("Cannot initialize command from {Handler}'s method {Name} - {Attribute} missing", handlerType.FullName, method.Name, nameof(CommandAttributeBase));
                return;
            }
            foreach (CommandAttributeBase attribute in attributes)
            {
                // ensure there's a valid initializer
                ICommandInitializer initializer = _initializers.GetMappedInitializer(attribute.GetType());
                if (initializer == null)
                    throw new InvalidOperationException($"No initializer found for command type {attribute.GetType().Name}");

                // check if handler is meant to be pre-initialized. If so, request it from provider to pre-initialize
                CommandHandlerAttribute handlerAttribute = method.DeclaringType.GetCustomAttribute<CommandHandlerAttribute>(true);
                if (handlerAttribute?.PreInitialize == true)
                {
                    _log?.LogDebug("Pre-initializing command handler {Handler}", handlerType.Name);
                    _handlerProvider.GetCommandHandler(attribute, method.DeclaringType);
                }

                // add the command
                _commands.Add((attribute, method));
                _log?.LogTrace("Command {Name} from handler {Handler} loaded", method.Name, handlerType.Name);
            }
        }

        private async void OnMessageReceived(ChatMessage message)
        {
            IEnumerable<(CommandAttributeBase, MethodInfo)> commandsCopy;
            await this._lock.WaitAsync(this.CancellationToken).ConfigureAwait(false);
            try
            {
                 commandsCopy = _commands.ToArray();
            }
            finally
            {
                this._lock.Release();
            }

            ICommandContext context = new CommandContext(message, this._client, this._options);
            foreach ((CommandAttributeBase attribute, MethodInfo method) in commandsCopy)
            {
                using (_log.BeginCommandScope(context, method.DeclaringType, method.Name))
                {
                    try
                    {
                        object handler = _handlerProvider.GetCommandHandler(attribute, method.DeclaringType);
                        ICommandInitializer initializer = _initializers.GetMappedInitializer(attribute.GetType());
                        ICommandInstance instance = initializer.InitializeCommand(attribute, method, handler);
                        ICommandResult checkResult = await instance.CheckShouldRunAsync(context, this.CancellationToken).ConfigureAwait(false);
                        if (!checkResult.IsSuccess)
                            continue;
                        _log?.LogTrace("Executing command {Name}", method.Name);
                        ICommandResult executeResult = await instance.ExecuteAsync(context, _services, checkResult, this.CancellationToken).ConfigureAwait(false);
                        if (!executeResult.IsSuccess)
                            _log?.LogError("Execution of command {Name} has failed", method.Name);
                        break;
                    }
                    catch (OperationCanceledException)
                    {
                        _log?.LogWarning("Execution of command {Name} was cancelled", method.Name);
                        return;
                    }
                    catch (Exception ex) when (ex.LogAsError(_log, "Unhandled Exception when executing command {Name}", method.Name)) { return; }
                }
            }
        }

        public void Dispose()
        {
            try { _lock?.Dispose(); } catch { }
            this._client.RemoveMessageListener<ChatMessage>(OnMessageReceived);
        }
    }
}
