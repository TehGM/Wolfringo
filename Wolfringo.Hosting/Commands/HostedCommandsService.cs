﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
#if !NETCOREAPP3_0_OR_GREATER
using Microsoft.AspNetCore.Hosting;
#endif
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TehGM.Wolfringo.Commands;
using TehGM.Wolfringo.Commands.Initialization;
using TehGM.Wolfringo.Utilities;

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
#if NETCOREAPP3_0_OR_GREATER
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
        private readonly IOptionsMonitor<CommandsOptions> _options;
        private readonly IServiceProvider _services;

        /// <inheritdoc/>
        public IEnumerable<ICommandInstanceDescriptor> Commands => this._commands.Commands;

        /// <summary>Creates a new hosted commands service.</summary>
        /// <param name="options">Commands options that will be used as default when running a command.</param>
        /// <param name="services">Services provider that will be used by all commands.</param>
        /// <param name="hostLifetime">Host lifetime that will be used to dispose service when application is exiting.</param>
        /// <param name="log">Logger used by hosted commands service.</param>
        public HostedCommandsService(IOptionsMonitor<CommandsOptions> options, IServiceProvider services, ILogger<HostedCommandsService> log,
#if NETCOREAPP3_0_OR_GREATER
            IHostApplicationLifetime hostLifetime
#else
            IApplicationLifetime hostLifetime
#endif
            )
        {
            this._options = options;
            this._services = services;
            this._log = log;
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
            this._commands = new CommandsService(this._services, this._options.CurrentValue);
        }

        /// <inheritdoc/>
        Task<ICommandResult> ICommandsService.ExecuteAsync(ICommandContext context, CancellationToken cancellationToken)
            => this._commands.ExecuteAsync(context, cancellationToken);

        /// <inheritdoc/>
        async Task ICommandsService.StartAsync(CancellationToken cancellationToken)
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
