using System;
using System.Threading;
using System.Threading.Tasks;
#if !NETCOREAPP3_0
using Microsoft.AspNetCore.Hosting;
#endif
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TehGM.Wolfringo.Commands;
using TehGM.Wolfringo.Commands.Initialization;
using TehGM.Wolfringo.Commands.Parsing;
using TehGM.Wolfringo.Utilities.Internal;

namespace TehGM.Wolfringo.Hosting.Commands
{
    /// <summary>A wrapper for <see cref="CommandsService"/> designed to use with .NET Core Host.</summary>
    /// <remarks><para>This wrapper uses <see cref="CommandsService"/> internally, and delegates all method calls and events.</para>
    /// <para>This class supports configuration changes by using <see cref="IOptionsMonitor{TOptions}"/>.
    /// Whenever settings are changed, existing commands service is disposed, and a new one is constructed.</para></remarks>
    /// <seealso cref="CommandsService"/>
    /// <seealso cref="ICommandsService"/>
    public class HostedCommandsService : ICommandsService, IHostedService, IDisposable
    {
#if NETCOREAPP3_0
        private readonly IHostApplicationLifetime _hostLifetime;
#else
        private readonly IApplicationLifetime _hostLifetime;
#endif
        private bool _isStarted;
        private CancellationToken _hostCancellationToken;
        private CommandsService _commands;
        private readonly SemaphoreSlim _commandsLock;
        private readonly IDisposable _exitingEventRegistration;
        private readonly IDisposable _optionsChangeEventRegistration;
        private readonly ILogger _log;

        // services for underlying commands service
        private readonly IWolfClient _client;
        private readonly IOptionsMonitor<CommandsOptions> _options;
        private readonly IServiceProvider _services;
        private readonly ICommandsHandlerProvider _handlerProvider;
        private readonly ICommandInitializerProvider _initializers;
        private readonly ICommandsLoader _commandsLoader;
        private readonly IArgumentsParser _argumentsParser;
        private readonly IArgumentConverterProvider _argumentConverterProvider;
        private readonly ILogger _underlyingServiceLog;

        /// <summary>Creates a new hosted commands service.</summary>
        /// <param name="client">WOLF client.</param>
        /// <param name="options">Commands options that will be used as default when running a command.</param>
        /// <param name="services">Services provider that will be used by all commands.</param>
        /// <param name="handlerProvider">Handler provider that deals with creation and caching of handler objects.</param>
        /// <param name="initializers">Map of command initializers for each command attribute.</param>
        /// <param name="commandsLoader">Service that loads command attributes from assemblies and types.</param>
        /// <param name="argumentsParser">Parser for the command arguments.</param>
        /// <param name="argumentConverterProvider">Provider of argument converters.</param>
        /// <param name="underlyingServiceLog">Logger that will be passed to underlying commands service.</param>
        /// <param name="hostLifetime">Host lifetime that will be used to dispose service when application is exiting.</param>
        /// <param name="log">Logger used by hosted commands service.</param>
        public HostedCommandsService(IWolfClient client, IOptionsMonitor<CommandsOptions> options, IServiceProvider services, ICommandsHandlerProvider handlerProvider, ICommandInitializerProvider initializers, ICommandsLoader commandsLoader, IArgumentsParser argumentsParser, IArgumentConverterProvider argumentConverterProvider, ILogger<HostedCommandsService> log, ILogger<CommandsService> underlyingServiceLog,
#if NETCOREAPP3_0
            IHostApplicationLifetime hostLifetime
#else
            IApplicationLifetime hostLifetime
#endif
            )
        {
            this._client = client;
            this._options = options;
            this._services = services;
            this._handlerProvider = handlerProvider;
            this._initializers = initializers;
            this._commandsLoader = commandsLoader;
            this._argumentsParser = argumentsParser;
            this._argumentConverterProvider = argumentConverterProvider;
            this._log = log;
            this._underlyingServiceLog = underlyingServiceLog ?? this._log;
            this._hostLifetime = hostLifetime;

            this._commandsLock = new SemaphoreSlim(1, 1);

            // dispose when closing
            this._exitingEventRegistration = this._hostLifetime.ApplicationStopping.Register(this.DisposeCommandsService);

            // when options change, recreate the service
            this._optionsChangeEventRegistration = this._options.OnChange(async (opts) =>
            {
                // lock to avoid race conditions
                await this._commandsLock.WaitAsync(this._hostCancellationToken).ConfigureAwait(false);
                try
                {
                    if (_commands == null)
                        return;
                    _log?.LogDebug("Options changed, recreating commands service");
                    this.CreateCommandsService();
                    await this.StartInternalAsync(this._hostCancellationToken).ConfigureAwait(false);
                }
                catch (OperationCanceledException) { }
                finally
                {
                    this._commandsLock.Release();
                }
            });
        }

        private void CreateCommandsService()
        {
            this.DisposeCommandsService();

            this._log?.LogTrace("Creating underlying commands service");
            this._commands = new CommandsService(
                this._client,
                this._options.CurrentValue,
                this._services,
                this._handlerProvider,
                this._initializers,
                this._commandsLoader,
                this._argumentsParser,
                this._argumentConverterProvider,
                this._underlyingServiceLog,
                this._hostCancellationToken);
        }

        /// <inheritdoc/>
        Task<ICommandResult> ICommandsService.ExecuteAsync(ICommandContext context, CancellationToken cancellationToken = default)
            => this._commands.ExecuteAsync(context, cancellationToken);

        /// <inheritdoc/>
        async Task ICommandsService.StartAsync(CancellationToken cancellationToken = default)
        {
            using (CancellationTokenSource startingCts = CancellationTokenSource.CreateLinkedTokenSource(_hostCancellationToken, cancellationToken))
            {
                await this._commandsLock.WaitAsync(startingCts.Token).ConfigureAwait(false);
                try
                {
                    await StartInternalAsync(startingCts.Token).ConfigureAwait(false);
                }
                finally
                {
                    this._commandsLock.Release();
                }
            }
        }

        private Task StartInternalAsync(CancellationToken cancellationToken = default)
        {
            CreateCommandsService();
            return this._commands.StartAsync(cancellationToken);
        }

        /// <inheritdoc/>
        async Task IHostedService.StartAsync(CancellationToken cancellationToken)
        {
            await this._commandsLock.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                if (this._isStarted)
                    return;
                this._isStarted = true;
                this._hostCancellationToken = cancellationToken;
                await this.StartInternalAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex.LogAsCritical(this._log, "Exception occured when starting hosted commands service")) { }
            finally
            {
                this._commandsLock.Release();
            }
        }

        /// <inheritdoc/>
        Task IHostedService.StopAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;

        private void DisposeCommandsService()
        {
            if (this._commands == null)
                return;
            this._log?.LogTrace("Disposing underlying commands service");
            try { this._commands?.Dispose(); } catch { }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.DisposeCommandsService();
            try { this._commandsLock.Dispose(); } catch { }
            try { this._exitingEventRegistration.Dispose(); } catch { }
            try { this._optionsChangeEventRegistration.Dispose(); } catch { }
        }
    }
}
