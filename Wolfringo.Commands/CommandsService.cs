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
        private readonly ICommandsLoader _commandsLoader;
        private readonly ILogger _log;

        public CancellationToken CancellationToken { get; set; }

        private ICollection<ICommandInstanceDescriptor> _commands;
        private readonly SemaphoreSlim _lock;
        private readonly IDictionary<ICommandInstanceDescriptor, ICommandInstance> _cachedInstances;

        public CommandsService(IWolfClient client, CommandsOptions options, IServiceProvider services = null, ICommandHandlerProvider handlerProvider = null, ICommandInitializerMap initializers = null,
            ICommandsLoader commandsLoader = null, ILogger log = null, CancellationToken cancellationToken = default)
        {
            // init required
            this._client = client ?? throw new ArgumentNullException(nameof(client));
            this._options = options ?? throw new ArgumentNullException(nameof(options));

            // init optionals
            this._log = log;
            this._services = services ?? this.CreateDefaultServiceProvider();
            this._handlerProvider = handlerProvider ?? new CommandHandlerProvider(this._services);
            this._initializers = initializers ?? new DefaultCommandInitializerMap();
            this._commandsLoader = commandsLoader ?? new DefaultCommandsLoader(this._initializers, this._log);
            this.CancellationToken = cancellationToken;

            // init private
            this._commands = new List<ICommandInstanceDescriptor>();
            this._lock = new SemaphoreSlim(1, 1);
            this._cachedInstances = new Dictionary<ICommandInstanceDescriptor, ICommandInstance>();

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
                    this._cachedInstances.Clear();

                    // ask loader to load from all specified assemblies and types
                    IEnumerable<ICommandInstanceDescriptor> descriptors = await _commandsLoader.LoadFromAssembliesAsync(_options.Assemblies ?? Enumerable.Empty<Assembly>(), cts.Token).ConfigureAwait(false);
                    descriptors = descriptors.Union(await _commandsLoader.LoadFromTypesAsync(_options.Classes.Select(t => t.GetTypeInfo()) ?? Enumerable.Empty<TypeInfo>(), cts.Token).ConfigureAwait(false));

                    // make sure there's no duplicates
                    descriptors = descriptors.Distinct();

                    // for each loaded command, handle pre-initialization and caching
                    foreach (ICommandInstanceDescriptor descriptor in descriptors)
                    {
                        CommandHandlerAttribute handlerAttribute = descriptor.GetHandlerAttribute();

                        // check if handler is meant to be pre-initialized. If so, request it from provider to pre-initialize
                        if (handlerAttribute?.PreInitialize == true)
                        {
                            _log?.LogDebug("Pre-initializing command handler {Handler}", descriptor.Method.DeclaringType.Name);
                            _handlerProvider.GetCommandHandler(descriptor);
                        }

                        // for performance: pre-create and cache command instance if it's a persistent one anyway
                        if (handlerAttribute?.IsPersistent == true)
                        {
                            _log?.LogDebug("Pre-creating command instance {Name} from handler {Handler}", descriptor.Method.Name, descriptor.Method.DeclaringType.Name);
                            _cachedInstances.Add(descriptor, CreateCommandInstance(descriptor));
                        }
                    }

                    // order according to priority and put it into commands storage
                    this._commands = descriptors.OrderByDescending(c => c.GetPriority()).ToArray();
                    this._log?.LogDebug("{Count} commands loaded", _commands.Count);
                }
                finally
                {
                    this._lock.Release();
                }
            }
        }

        private async void OnMessageReceived(ChatMessage message)
        {
            // make a copy - this might not be the fastest, but we should use a lock to prevent race conditions with StartAsync
            // and using a lock over the entire method could cause hanging if user is not carefull in their command method. Copying is just safer
            IEnumerable<ICommandInstanceDescriptor> commandsCopy;
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
            foreach (ICommandInstanceDescriptor command in commandsCopy)
            {
                using (_log.BeginCommandScope(context, command.Method.DeclaringType, command.Method.Name))
                {
                    try
                    {
                        // try to get instance from cache - or create if it's not there
                        if (!_cachedInstances.TryGetValue(command, out ICommandInstance instance))
                            instance = CreateCommandInstance(command);

                        // check if the command should run at all - if not, skip
                        ICommandResult checkResult = await instance.CheckShouldRunAsync(context, this.CancellationToken).ConfigureAwait(false);
                        if (!checkResult.IsSuccess)
                            continue;

                        // execute the command
                        _log?.LogTrace("Executing command {Name} from handler {Handler}", command.Method.Name, command.Method.DeclaringType.Name);
                        ICommandResult executeResult = await instance.ExecuteAsync(context, _services, checkResult, this.CancellationToken).ConfigureAwait(false);
                        if (!executeResult.IsSuccess)
                            _log?.LogError("Execution of command {Name} from handler {Handler} has failed", command.Method.Name, command.Method.DeclaringType.Name);
                        break;
                    }
                    catch (OperationCanceledException)
                    {
                        _log?.LogWarning("Execution of command {Name} from handler {Handler} was cancelled", command.Method.Name, command.Method.DeclaringType.Name);
                        return;
                    }
                    catch (Exception ex) when (ex.LogAsError(_log, "Unhandled Exception when executing command {Name} from handler {Handler}", command.Method.Name, command.Method.DeclaringType.Name)) { return; }
                }
            }
        }

        private ICommandInstance CreateCommandInstance(ICommandInstanceDescriptor descriptor)
        {
            object handler = _handlerProvider.GetCommandHandler(descriptor);
            ICommandInitializer initializer = _initializers.GetMappedInitializer(descriptor.Attribute.GetType());
            return initializer.InitializeCommand(descriptor, handler, _options);
        }

        public void Dispose()
        {
            this._cachedInstances?.Clear();
            this._commands?.Clear();
            try { _lock?.Dispose(); } catch { }
            this._client.RemoveMessageListener<ChatMessage>(OnMessageReceived);
        }
    }
}
