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

        private List<ICommandInstance> _commands;
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
            this._commands = new List<ICommandInstance>();
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
            IEnumerable<TypeInfo> types = assembly.DefinedTypes.Where(t => !t.IsAbstract && !t.ContainsGenericParameters
                && !Attribute.IsDefined(t, typeof(CompilerGeneratedAttribute)) && Attribute.IsDefined(t, typeof(CommandHandlerAttribute)));
            if (!types.Any())
            {
                _log?.LogWarning("Cannot initialize commands from assembly {AssemblyName} - no non-static non-abstract non-generic classes with {Attribute}", assembly.FullName, nameof(CommandHandlerAttribute));
                return;
            }
            foreach (TypeInfo type in types)
                AddType(type);
        }

        private void AddType(TypeInfo type)
        {
            IEnumerable<MethodInfo> methods = type.DeclaredMethods.Where(m => !m.IsStatic && !Attribute.IsDefined(m, typeof(CompilerGeneratedAttribute)) && Attribute.IsDefined(m, typeof(CommandAttributeBase)));
            if (!methods.Any())
            {
                _log?.LogWarning("Cannot initialize commands from type {TypeName} - no method with {Attribute}", type.FullName, nameof(CommandAttributeBase));
                return;
            }
            foreach (MethodInfo method in methods)
                AddMethod(method);
        }

        private void AddMethod(MethodInfo method)
        {
            IEnumerable<CommandAttributeBase> attributes = method.GetCustomAttributes<CommandAttributeBase>();
            if (!attributes.Any())
            {
                _log?.LogWarning("Cannot initialize command from {TypeName}'s method {MethodName} - {Attribute} missing", method.DeclaringType.FullName, method.Name, nameof(CommandAttributeBase));
                return;
            }
            foreach (CommandAttributeBase attribute in attributes)
            {
                ICommandInitializer initializer = _initializers.GetMappedInitializer(attribute.GetType());
                if (initializer == null)
                    throw new InvalidOperationException($"No initializer found for command type {attribute.GetType().Name}");
                object handler = _handlerProvider.GetCommandHandler(attribute, method.DeclaringType);
                ICommandInstance command = initializer.InitializeCommand(attribute, method, handler);
                _commands.Add(command);
            }
        }

        private async void OnMessageReceived(ChatMessage message)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            try { _lock?.Dispose(); } catch { }
            this._client.RemoveMessageListener<ChatMessage>(OnMessageReceived);
        }
    }
}
