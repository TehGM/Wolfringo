using System;
using System.Threading;
using System.Threading.Tasks;

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
        /// <summary>How much time to wait before reconnecting.</summary>
        /// <remarks><para>Set to 0 or negative value to reconnect instantly.</para>
        /// <para>Defaults to 500ms.</para></remarks>
        public TimeSpan ReconnectionDelay { get; set; } = TimeSpan.FromSeconds(0.5);

        private readonly IWolfClient _client;
        private readonly CancellationToken _cancellationToken;

        /// <summary>Raised when client fails to reconnect.</summary>
        /// <remarks>The object will be disposed after this event executes.</remarks>
        public event EventHandler<UnhandledExceptionEventArgs> FailedToReconnect;

        /// <summary>Creates</summary>
        /// <param name="client"></param>
        /// <param name="connectionCancellationToken"></param>
        public WolfClientReconnector(IWolfClient client, CancellationToken connectionCancellationToken = default)
        {
            this._client = client;
            this._cancellationToken = connectionCancellationToken;
            this._client.Disconnected += OnClientDisconnected;
        }

        /// <summary>Reconnects the client.</summary>
        /// <remarks>This method is invoked when the client has disconnected.</remarks>
        private async void OnClientDisconnected(object sender, EventArgs e)
        {
            try
            {
                // wait reconnection delay if any
                if (this.ReconnectionDelay > TimeSpan.Zero)
                    await Task.Delay(this.ReconnectionDelay);
                // attempt to reconnnect unconditionally
                await _client.ConnectAsync(_cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // on error, raise event and dispose the instance.
                FailedToReconnect?.Invoke(this, new UnhandledExceptionEventArgs(ex, true));
                this.Dispose();
            }
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
