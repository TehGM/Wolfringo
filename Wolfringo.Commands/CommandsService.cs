using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TehGM.Wolfringo.Commands.Initialization;
using TehGM.Wolfringo.Messages;
using TehGM.Wolfringo.Utilities.Internal;

namespace TehGM.Wolfringo.Commands
{
    /// <summary>A service that deals with commands loading, initialization and execution.</summary>
    /// <remarks><para>This is a base service that runs the commands. It'll manage all other parts of Commands System.</para>
    /// <para>This command service can be customized partially by injecting custom services into its constructor. If these services are set to default or skipped, default instances will be automatically created and used, similarly to <see cref="WolfClient"/>.</para></remarks>
    public class CommandsService : IDisposable
    {
        private readonly IWolfClient _client;
        private readonly CommandsOptions _options;
        private readonly IServiceProvider _services;
        private readonly ICommandHandlerProvider _handlerProvider;
        private readonly ICommandInitializerMap _initializers;
        private readonly ICommandsLoader _commandsLoader;
        private readonly ILogger _log;
        private readonly CancellationTokenSource _cts;

        // TODO: using of CTS as it is kills and point of this. Redesign.
        public CancellationToken CancellationToken { get; set; }

        private ICollection<ICommandInstanceDescriptor> _commands;
        private List<IDisposable> _disposables;
        private readonly SemaphoreSlim _lock;
        private readonly IDictionary<ICommandInstanceDescriptor, ICommandInstance> _cachedInstances;

        /// <summary>Initializes a command service.</summary>
        /// <param name="client">WOLF client. Required.</param>
        /// <param name="options">Commands options that will be used as default when running a command. Required.</param>
        /// <param name="services">Services provider that will be used by all commands. Null will cause a default to be used.</param>
        /// <param name="handlerProvider">Handler provider that deals with creation and caching of handler objects. Null will cause a default to be used.</param>
        /// <param name="initializers">Map of command initializers for each command attribute. Null will cause a default to be used.</param>
        /// <param name="commandsLoader">Service that loads command attributes from assemblies and types. Null will cause a default to be used.</param>
        /// <param name="log">Logger to log messages and errors to. If null, all logging will be disabled.</param>
        /// <param name="cancellationToken">Cancellation token that can be used for cancelling all tasks. If not provided, task cancellation will not be possible.</param>
        public CommandsService(IWolfClient client, CommandsOptions options, IServiceProvider services = null, ICommandHandlerProvider handlerProvider = null, ICommandInitializerMap initializers = null, ICommandsLoader commandsLoader = null, ILogger log = null, CancellationToken cancellationToken = default)
        {
            // init required
            this._client = client ?? throw new ArgumentNullException(nameof(client));
            this._options = options ?? throw new ArgumentNullException(nameof(options));

            // init optionals
            this._log = log;
            this._services = services ?? this.CreateDefaultServiceProvider();
            this._handlerProvider = handlerProvider;
            if (this._handlerProvider == null)
            {
                this._handlerProvider = new CommandHandlerProvider(this._services);
                this._disposables.Add(this._handlerProvider as IDisposable);
            }
            this._initializers = initializers ?? new DefaultCommandInitializerMap();
            this._commandsLoader = commandsLoader ?? new DefaultCommandsLoader(this._initializers, this._log);
            this.CancellationToken = cancellationToken;

            // init private
            this._commands = new List<ICommandInstanceDescriptor>();
            this._lock = new SemaphoreSlim(1, 1);
            this._cachedInstances = new Dictionary<ICommandInstanceDescriptor, ICommandInstance>();
            this._cts = CancellationTokenSource.CreateLinkedTokenSource(this.CancellationToken);

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

        /// <summary>Starts the Command Service.</summary>
        /// <param name="cancellationToken">Cancellation token to cancel loading with.</param>
        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            using (CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cts.Token))
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
                            _log?.LogDebug("Pre-initializing command handler {Handler}", descriptor.GetHandlerType().Name);
                            _handlerProvider.GetCommandHandler(descriptor);
                        }

                        // for performance: pre-create and cache command instance if it's a persistent one anyway
                        if (handlerAttribute?.IsPersistent == true)
                        {
                            _log?.LogDebug("Pre-creating command instance {Name} from handler {Handler}", descriptor.Method.Name, descriptor.GetHandlerType().Name);
                            ICommandInstance instance = CreateCommandInstance(descriptor);
                            _cachedInstances.Add(descriptor, instance);
                            if (instance is IDisposable disposableInstance)
                                this._disposables.Add(disposableInstance);
                        }
                    }

                    // grab all Disposable command descriptors (if any) for future disposing
                    this._disposables.AddRange(descriptors.Where(c => c is IDisposable).Cast<IDisposable>());

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
            ICommandInstanceDescriptor[] commandsCopy = new ICommandInstanceDescriptor[_commands.Count];
            await this._lock.WaitAsync(this._cts.Token).ConfigureAwait(false);
            try
            {
                _commands.CopyTo(commandsCopy, 0);
            }
            finally
            {
                this._lock.Release();
            }

            ICommandContext context = new CommandContext(message, this._client, this._options);
            foreach (ICommandInstanceDescriptor command in commandsCopy)
            {
                using (_log.BeginCommandScope(context, command.GetHandlerType().Name, command.Method.Name))
                {
                    ICommandHandlerProviderResult handlerResult = null;
                    try
                    {
                        // try to get instance from cache - or create if it's not there
                        if (!_cachedInstances.TryGetValue(command, out ICommandInstance instance))
                            instance = CreateCommandInstance(command);

                        // check if the command should run at all - if not, skip
                        ICommandResult matchResult = await instance.CheckMatchAsync(context, this._services, this._cts.Token).ConfigureAwait(false);
                        if (!matchResult.IsSuccess)
                            continue;

                        // execute the command
                        _log?.LogTrace("Executing command {Name} from handler {Handler}", command.Method.Name, command.GetHandlerType().Name);
                        handlerResult = _handlerProvider.GetCommandHandler(command);
                        ICommandResult executeResult = await instance.ExecuteAsync(context, _services, matchResult, handlerResult, this._cts.Token).ConfigureAwait(false);
                        if (!executeResult.IsSuccess)
                            _log?.LogError("Execution of command {Name} from handler {Handler} has failed", command.Method.Name, command.Method.DeclaringType.Name);
                        break;
                    }
                    catch (OperationCanceledException)
                    {
                        _log?.LogWarning("Execution of command {Name} from handler {Handler} was cancelled", command.Method.Name, command.GetHandlerType().Name);
                        return;
                    }
                    catch (Exception ex) when (ex.LogAsError(_log, "Unhandled Exception when executing command {Name} from handler {Handler}", command.Method.Name, command.GetHandlerType().Name)) { return; }
                    finally
                    {
                        // if handler is allocated, not persistent and disposable, let's dispose it
                        if (handlerResult.Descriptor.Attribute.IsPersistent && handlerResult.HandlerInstance is IDisposable disposableHandler)
                            try { disposableHandler?.Dispose(); } catch { }
                    }
                }
            }
        }

        private ICommandInstance CreateCommandInstance(ICommandInstanceDescriptor descriptor)
        {
            ICommandInitializer initializer = _initializers.GetMappedInitializer(descriptor.Attribute.GetType());
            return initializer.InitializeCommand(descriptor, _options);
        }

        /// <summary>Disposes the Command Service.</summary>
        /// <remarks><para>This method will dispose services created by default if they weren't provided via the constructor. If service was provided via constructor, it will NOT be disposed - please dispose it manually when convenient.</para>
        /// <para>This method will also dispose all command instances and descriptors that happen to implement <see cref="IDisposable"/>.</para></remarks>
        public void Dispose()
        {
            // remove event listener
            this._client.RemoveMessageListener<ChatMessage>(OnMessageReceived);
            // cancel all tasks
            try { this._cts?.Cancel(); } catch { }
            try { this._cts?.Dispose(); } catch { }
            // purge collections
            this._cachedInstances?.Clear();
            this._commands?.Clear();
            // dispose all objects (such as instances, descriptors, or services) that implement IDisposable
            foreach (IDisposable disposable in this._disposables)
                try { disposable?.Dispose(); } catch { }
            this._disposables.Clear();
            // dispose semaphore
            try { _lock?.Dispose(); } catch { }
        }
    }
}
