﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TehGM.Wolfringo.Commands.Initialization;
using TehGM.Wolfringo.Commands.Results;
using TehGM.Wolfringo.Messages;
using TehGM.Wolfringo.Utilities;
using TehGM.Wolfringo.Commands.Parsing;
using Microsoft.Extensions.DependencyInjection;
using TehGM.Wolfringo.Messages.Responses;
using TehGM.Wolfringo.Utilities.Internal;

namespace TehGM.Wolfringo.Commands
{
    /// <summary>A service that deals with commands loading, initialization and execution.</summary>
    /// <remarks><para>This is a default service that runs the commands. It'll manage all other parts of Commands System.</para>
    /// <para>This command service can be customized partially by injecting custom services into its constructor. If these services are set to default or skipped, default instances will be automatically created and used, similarly to <see cref="WolfClient"/>.</para>
    /// <para>Constructor takes <see cref="IServiceProvider"/> as one of params. Services contained in that provider will be used by default. If service cannot be resolved, or the provider is null, a fallback provider will be used.<br/>
    /// This hierarchy is used through entire command execution, so custom provider does not need to specify services required by Commands Service.<br/>
    /// Fallback provider will create services only if they are not provided via custom provider.<br/>
    /// Services injected via custom provider will NOT be disposed when <see cref="Dispose"/> is invoked. Please dispose them manually.</para>
    /// </remarks>
    public class CommandsService : ICommandsService, IDisposable
    {
        private readonly IWolfClient _client;
        private readonly CommandsOptions _options;
        private readonly IServiceProvider _services;
        private readonly ICommandsHandlerProvider _handlerProvider;
        private readonly ICommandInitializerProvider _initializers;
        private readonly ICommandsLoader _commandsLoader;
        private readonly IArgumentsParser _argumentsParser;
        private readonly IParameterBuilder _parameterBuilder;
        private readonly IArgumentConverterProvider _argumentConverterProvider;
        private readonly ILogger _log;
        private readonly CancellationTokenSource _cts;

        private readonly DisposableServicesHandler _disposablesHandler;
        private bool _started;
        private readonly SemaphoreSlim _lock;
        private readonly IDictionary<ICommandInstanceDescriptor, ICommandInstance> _commands;

        /// <inheritdoc/>
        public IEnumerable<ICommandInstanceDescriptor> Commands => this._commands.Keys;

        /// <summary>Initializes a command service.</summary>
        /// <param name="client">WOLF client. Required.</param>
        /// <param name="options">Commands options that will be used as default when running a command. Required.</param>
        [Obsolete("Use CommandsServiceBuilder instead")]
        public CommandsService(IWolfClient client, CommandsOptions options)
            : this(client, options, null) { }

        /// <summary>Initializes a command service.</summary>
        /// <param name="client">WOLF client. Required.</param>
        /// <param name="options">Commands options that will be used as default when running a command. Required.</param>
        /// <param name="log">Logger to log messages and errors to. If null, all logging will be disabled.</param>
        [Obsolete("Use CommandsServiceBuilder instead")]
        public CommandsService(IWolfClient client, CommandsOptions options, ILogger log)
            : this(BuildDefaultServiceProvider(client, options, log), options) { }

        /// <summary>Initializes a command service.</summary>
        /// <param name="services">Service provider to resolve dependencies from</param>
        /// <param name="options">Commands options to use for all commands.</param>
        public CommandsService(IServiceProvider services, CommandsOptions options)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            // init private
            this._options = options;
            this._commands = new Dictionary<ICommandInstanceDescriptor, ICommandInstance>();
            this._lock = new SemaphoreSlim(1, 1);
            this._cts = CancellationTokenSource.CreateLinkedTokenSource(this._options.CancellationToken);
            this._disposablesHandler = services.GetService<DisposableServicesHandler>() ?? new DisposableServicesHandler();
            this._started = false;
            this._services = services;

            // init services
            this._client = this._disposablesHandler.GetRequiredService<IWolfClient>(services);
            this._argumentConverterProvider = this._disposablesHandler.GetRequiredService<IArgumentConverterProvider>(services);
            this._handlerProvider = this._disposablesHandler.GetRequiredService<ICommandsHandlerProvider>(services);
            this._argumentsParser = this._disposablesHandler.GetRequiredService<IArgumentsParser>(services);
            this._parameterBuilder = this._disposablesHandler.GetRequiredService<IParameterBuilder>(services);
            this._initializers = this._disposablesHandler.GetRequiredService<ICommandInitializerProvider>(services);
            this._commandsLoader = this._disposablesHandler.GetRequiredService<ICommandsLoader>(services);
            this._log = services.GetService<ILogger<CommandsService>>()
                ?? services.GetService<ILogger<ICommandsService>>()
                ?? services.GetService<ILogger>()
                ?? services.GetService<ILoggerFactory>()?.CreateLogger<CommandsService>();

            // register event handlers
            this._client.AddMessageListener<ChatMessage>(OnMessageReceived);
        }

        /// <summary>Builds default service provider. Used to temporarily support obsolete non-builder constructors.</summary>
        /// <param name="client">Wolf Client to use with CommandsService.</param>
        /// <param name="options">Options for commands service.</param>
        /// <param name="log">A logger to add to the services. If null, logging will be disabled.</param>
        /// <returns>A <see cref="IServiceProvider"/> with default services added.</returns>
        [Obsolete]
        protected static IServiceProvider BuildDefaultServiceProvider(IWolfClient client, CommandsOptions options, ILogger log = null)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            // add all required services
            IServiceCollection services = new ServiceCollection();
            services.AddSingleton<IWolfClient>(client);
            services.AddSingleton(client.GetType(), client);
            services.AddSingleton<CommandsOptions>(options);
            services.AddSingleton<ICommandOptions>(provider => provider.GetRequiredService<CommandsOptions>());
            services.AddTransient<IArgumentsParser, ArgumentsParser>();
            services.AddTransient<IParameterBuilder, ParameterBuilder>();
            services.AddSingleton<IArgumentConverterProvider, ArgumentConverterProvider>();
            services.AddSingleton<ICommandsHandlerProvider, CommandsHandlerProvider>();
            services.AddSingleton<ICommandInitializerProvider, CommandInitializerProvider>();
            services.AddTransient<ICommandsLoader>(provider 
                => new CommandsLoader(provider.GetRequiredService<ICommandInitializerProvider>(), 
                provider.GetLoggerFor<CommandsLoader>()));

            if (log != null)
            {
                if (log is ILogger<CommandsService> typedLog)
                    services.AddSingleton<ILogger<CommandsService>>(typedLog);
                else if (log is ILogger<ICommandsService> interfaceTypedLog)
                    services.AddSingleton<ILogger<ICommandsService>>(interfaceTypedLog);
                services.AddSingleton<ILogger>(log);
            }

            // add tracker to know to dispose them
            DisposableServicesHandler handler = new DisposableServicesHandler();
            handler.MarkForDisposal<IArgumentConverterProvider>();
            handler.MarkForDisposal<ICommandsHandlerProvider>();
            handler.MarkForDisposal<ICommandInitializerProvider>();
            services.AddSingleton<DisposableServicesHandler>(handler);

            return services.BuildServiceProvider();
        }

        /// <inheritdoc/>
        /// <remarks>Once already started, calling this method again will cause commands to be re-loaded.</remarks>
        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            using (CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, this._cts.Token))
            {
                await this._lock.WaitAsync(cts.Token).ConfigureAwait(false);
                try
                {
                    this._log?.LogDebug("Initializing commands");

                    // dispose commands since we're reloading them
                    this.DisposeCommands();

                    // ask loader to load from all specified assemblies and types
                    IEnumerable<ICommandInstanceDescriptor> descriptors = await _commandsLoader.LoadFromAssembliesAsync(_options.Assemblies ?? Enumerable.Empty<Assembly>(), cts.Token).ConfigureAwait(false);
                    descriptors = descriptors.Union(await _commandsLoader.LoadFromTypesAsync(_options.Classes.Select(t => t.GetTypeInfo()) ?? Enumerable.Empty<TypeInfo>(), cts.Token).ConfigureAwait(false));

                    // add default help command if enabled
                    if (this._options.EnableDefaultHelpCommand)
                    {
                        this._log?.LogTrace("Default help command is enabled, loading");
                        descriptors = descriptors.Union(await _commandsLoader.LoadFromMethodAsync(
                            typeof(Help.DefaultHelpCommandHandler).GetMethod(nameof(Help.DefaultHelpCommandHandler.CmdHelpAsync), BindingFlags.Instance | BindingFlags.Public), cts.Token).ConfigureAwait(false));
                    }

                    // make sure there's no duplicates
                    descriptors = descriptors.Distinct();
                    
                    // for each loaded command, handle pre-initialization and caching
                    foreach (ICommandInstanceDescriptor descriptor in descriptors)
                    {
                        CommandsHandlerAttribute handlerAttribute = descriptor.GetHandlerAttribute();

                        // check if handler is persistent. If so, request it from provider to pre-initialize
                        if (handlerAttribute?.IsPersistent == true)
                        {
                            this._log?.LogDebug("Pre-initializing command handler {Handler}", descriptor.GetHandlerType().Name);
                            this._handlerProvider.GetCommandHandler(descriptor, this._services);
                        }

                        // create all command instances
                        this._log?.LogDebug("Creating command instance {Name} from handler {Handler}", descriptor.Method.Name, descriptor.GetHandlerType().Name);
                        ICommandInitializer initializer = this._initializers.GetInitializer(descriptor.Attribute.GetType());
                        ICommandInstance instance = initializer.InitializeCommand(descriptor, _options);
                        this._commands.Add(descriptor, instance);
                    }

                    // mark as started
                    this._started = true;
                    this._log?.LogDebug("{Count} commands loaded", _commands.Count);
                }
                finally
                {
                    this._lock.Release();
                }
            }
        }

        // 2 ways to handle errors:
        // if executed from OnMessageReceived event handler, we need to capture all errors and log them, otherwise they're lost - ExecuteAsyncInternal will log per-command scope, so we just need to catch whatever was outside
        // for the public ExecuteAsync, assume caller will handle exceptions on their own - for this reason, capture exception from ICommandResult, and rethrow it. Use ExceptionDispatchInfo to not lose stack trace

        private async void OnMessageReceived(ChatMessage message)
        {
            if (!this._started)
                return;
            try
            {
                ICommandContext context = new CommandContext(message, this._client, this._options);
                await ExecuteAsyncInternal(context).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex.LogAsError(this._log, "Unhandled exception when executing commands")) { }
        }

        /// <inheritdoc/>
        public async Task<ICommandResult> ExecuteAsync(ICommandContext context, CancellationToken cancellationToken = default)
        {
            ICommandResult result = await ExecuteAsyncInternal(context, cancellationToken).ConfigureAwait(false);
            if (result is CommandExecutionResult execResult && execResult.Exception != null)
                ExceptionDispatchInfo.Capture(execResult.Exception).Throw();
            return result;
        }

        private async Task<ICommandResult> ExecuteAsyncInternal(ICommandContext context, CancellationToken cancellationToken = default)
        {
            if (!this._started)
                throw new InvalidOperationException($"This {this.GetType().Name} is not started yet");
            using (CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, this._cts.Token))
            {
                using (IServiceScope serviceScope = this._services.CreateScope())
                {
                    IServiceProvider services = serviceScope.ServiceProvider;

                    foreach (KeyValuePair<ICommandInstanceDescriptor, ICommandInstance> commandKvp in this._commands.OrderByDescending(kvp => kvp.Key.GetPriority()))
                    {
                        ICommandInstanceDescriptor command = commandKvp.Key;
                        ICommandInstance instance = commandKvp.Value;
                        using (this._log.BeginCommandScope(context, command.GetHandlerType().Name, command.Method.Name))
                        {
                            ICommandsHandlerProviderResult handlerResult = null;
                            try
                            {
                                cts.Token.ThrowIfCancellationRequested();

                                // check if the command should run at all - if not, skip
                                ICommandResult matchResult = await instance.CheckMatchAsync(context, services, cts.Token).ConfigureAwait(false);
                                if (matchResult.Status != CommandResultStatus.Success)
                                    continue;

                                // initialize handler
                                this._log?.LogTrace("Initializing handler handler {Handler} for command {Name}", command.GetHandlerType().Name, command.Method.Name);
                                handlerResult = this._handlerProvider.GetCommandHandler(command, services);
                                if (handlerResult?.HandlerInstance == null)
                                {
                                    this._log?.LogError("Retrieving handler {Handler} for command {Name} has failed, command execution aborting", command.GetHandlerType().Name, command.Method.Name);
                                    return CommandExecutionResult.FromException(new ArgumentNullException(nameof(ICommandsHandlerProviderResult.HandlerInstance),
                                        $"Retrieving handler {command.GetHandlerType().Name} for command {command.Method.Name} has failed, command execution aborting"));
                                }
                                this._log?.LogTrace("Executing command {Name} from handler {Handler}", command.Method.Name, command.GetHandlerType().Name);

                                // execute the command
                                ICommandResult executeResult = await instance.ExecuteAsync(context, services, matchResult, handlerResult.HandlerInstance, cts.Token).ConfigureAwait(false);
                                if (executeResult is IMessagesCommandResult messagesResult && messagesResult.Messages?.Any() == true)
                                {
                                    this._log?.LogTrace("Sending command results messages as a command response");
                                    await context.ReplyTextAsync(string.Join("\n", messagesResult.Messages), cts.Token).ConfigureAwait(false);
                                }
                                if (executeResult.Status == CommandResultStatus.Skip)
                                    continue;
                                return executeResult;
                            }
                            // special error case: operation canceled
                            // operation canceled is normal, so it shouldn't be logged as error
                            catch (OperationCanceledException ex)
                            {
                                this._log?.LogWarning("Execution of command {Name} from handler {Handler} was cancelled", command.Method.Name, command.GetHandlerType().Name);
                                return CommandExecutionResult.FromException(ex);
                            }
                            // special error case: responding when silenced
                            // bots almost always respond to a command - but if they're silenced, an exception will be thrown
                            // this is normal - so it shouldn't be logged as error. Warning max
                            catch (MessageSendingException ex) when 
                            (this.LogSilencedException(ex, context, "Unhandled Exception when executing command {Name} from handler {Handler} - likely due to being silenced or spam filtered", command.Method.Name, command.GetHandlerType().Name))
                            {
                                return CommandExecutionResult.FromException(ex);
                            }
                            // normal error case
                            catch (Exception ex) when (ex.LogAsError(_log, "Unhandled Exception when executing command {Name} from handler {Handler}", command.Method.Name, command.GetHandlerType().Name))
                            {
                                return CommandExecutionResult.FromException(ex);
                            }
                            finally
                            {
                                // if handler is allocated, not persistent and disposable, let's dispose it
                                if (handlerResult?.Descriptor?.Attribute?.IsPersistent != true && handlerResult?.HandlerInstance is IDisposable disposableHandler)
                                    try { disposableHandler?.Dispose(); } catch { }
                            }
                        }
                    }
                }
                return CommandExecutionResult.Failure;
            }
        }

        private bool LogSilencedException(MessageSendingException ex, ICommandContext context, string message, params object[] args)
        {
            // ensure this is status code 403 with internal code 1
            if (ex.StatusCode != System.Net.HttpStatusCode.Forbidden)
                return false;
            if (!(ex.Response is WolfResponse wolfResponse))
                return false;
            if (wolfResponse.ErrorCode != WolfErrorCode.LoginIncorrectOrCannotSendMessage)
                return false;

            // only handle for sent chat messages
            if (!(ex.SentMessage is IChatMessage sentMessage))
                return false;

            // only handle if the recipient is the same as the command sender
            if (context.Message.IsGroupMessage)
            {
                if (!sentMessage.IsGroupMessage)
                    return false;
                if (sentMessage.RecipientID != context.Message.RecipientID)
                    return false;
            }
            else
            {
                if (sentMessage.IsGroupMessage)
                    return false;
                if (sentMessage.RecipientID != context.Message.SenderID.Value)
                    return false;
            }

            // log as warning
            this._log?.LogWarning(ex, message, args);
            return true;
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
            // dispose all command instances and descriptors that implement IDisposable
            this.DisposeCommands();
            this._disposablesHandler?.Dispose();
            // dispose semaphore
            try { this._lock?.Dispose(); } catch { }
        }

        private void DisposeCommands()
        {
            foreach (KeyValuePair<ICommandInstanceDescriptor, ICommandInstance> cmd in this._commands)
            {
                if (cmd.Key is IDisposable disposableDescriptor)
                    try { disposableDescriptor.Dispose(); } catch { }
                if (cmd.Value is IDisposable disposableInstance)
                    try { disposableInstance.Dispose(); } catch { }
            }
            this._commands.Clear();
        }
    }
}
