using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace TehGM.Wolfringo.Utilities
{
    /// <summary>Utility that will attempt to automatically reconnect the client on disconnection.</summary>
    /// <remarks><para>This utility will try yo reconnect on all disconnections. If you wish to stop this behaviour, dispose
    /// this object by calling <see cref="Dispose"/>. To re-enable, simply create a new instance again.</para>
    /// <para>In case a reconnection fails, <see cref="FailedToReconnect"/> will be raised, and the class will be disposed automatically.
    /// To re-enable, create a new instance.</para>
    /// <para>Any <see cref="IWolfClient"/> should have only one instance of this utility active at once.</para>
    /// <para>If using IHostedWolfClient from Wolfringo.Hosting package, do not use this utility. The default hosted
    /// client has auto-reconnecting behaviour built-in.</para></remarks>
    public class WolfClientReconnector : IDisposable
    {
        /// <summary>Configuration for reconnecting.</summary>
        public ReconnectorConfig Config { get; }

        private readonly IWolfClient _client;

        /// <summary>Raised when client fails to reconnect.</summary>
        /// <remarks>The object will be disposed after this event executes.</remarks>
        public event EventHandler<UnhandledExceptionEventArgs> FailedToReconnect;

        /// <summary>Creates instance of reconnector.</summary>
        /// <param name="client">Client to automatically reconnect.</param>
        /// <param name="config">Reconnector configuration.</param>
        public WolfClientReconnector(IWolfClient client, ReconnectorConfig config)
        {
            this._client = client ?? throw new ArgumentNullException(nameof(client));
            this.Config = config ?? throw new ArgumentNullException(nameof(config));
            this._client.Disconnected += OnClientDisconnected;
        }

        /// <summary>Creates instance of reconnector using default config values.</summary>
        /// <param name="client">Client to automatically reconnect.</param>
        public WolfClientReconnector(IWolfClient client) : this(client, new ReconnectorConfig()) { }

        /// <summary>Reconnects the client.</summary>
        /// <remarks>This method is invoked when the client has disconnected.</remarks>
        /// <exception cref="AggregateException">Aggregate exception containing all exceptions occured over all reconnect attempts.</exception>
        private async void OnClientDisconnected(object sender, EventArgs e)
        {
            this.Config.Log?.LogDebug("Attempting to reconnect, max {Attempts} times. Delay: {Delay}",
                this.Config.ReconnectAttempts, this.Config.ReconnectionDelay);

            ICollection<Exception> exceptions = new List<Exception>(this.Config.ReconnectAttempts);
            for (int i = 1; i <= this.Config.ReconnectAttempts; i++)
            {
                try
                {
                    this.Config.Log?.LogTrace("Reconnection attempt {Attempt}", i);

                    // wait reconnection delay if any
                    if (this.Config.ReconnectionDelay > TimeSpan.Zero)
                        await Task.Delay(this.Config.ReconnectionDelay);

                    // attempt to reconnnect unconditionally
                    await _client.ConnectAsync(this.Config.CancellationToken).ConfigureAwait(false);
                    return;
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }

            AggregateException aggrEx = exceptions.Any() ?
                new AggregateException("Error(s) occured when trying to automatically reconnect", exceptions) :
                new AggregateException("Failed to reconnect, but no exceptions were thrown");
            this.Config.Log?.LogError(aggrEx, "Failed to reconnect after {Attempts} attempts", this.Config.ReconnectAttempts);
            FailedToReconnect?.Invoke(this, new UnhandledExceptionEventArgs(aggrEx, true));
            throw aggrEx;
        }

        /// <summary>Disposes the instance by removing event handlers.</summary>
        /// <remarks>After this method is called, the utility will no longer automatically reconnect.
        /// To continue this behaviour, create a new instance of this utility.</remarks>
        public void Dispose()
        {
            this._client.Disconnected -= OnClientDisconnected;
        }
    }
}
