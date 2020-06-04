using System;
using System.Threading;
using System.Threading.Tasks;

namespace TehGM.Wolfringo.Utilities
{
    public class WolfClientReconnector : IDisposable
    {
        public TimeSpan ReconnectionDelay { get; set; } = TimeSpan.FromSeconds(0.5);

        private readonly IWolfClient _client;
        private readonly CancellationToken _cancellationToken;

        public event EventHandler<UnhandledExceptionEventArgs> FailedToReconnect;

        public WolfClientReconnector(IWolfClient client, CancellationToken connectionCancellationToken = default)
        {
            this._client = client;
            this._cancellationToken = connectionCancellationToken;
            this._client.Disconnected += OnClientDisconnected;
        }

        private async void OnClientDisconnected(object sender, EventArgs e)
        {
            try
            {
                if (this.ReconnectionDelay > TimeSpan.Zero)
                    await Task.Delay(this.ReconnectionDelay);
                await _client.ConnectAsync(_cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                FailedToReconnect?.Invoke(this, new UnhandledExceptionEventArgs(ex, true));
            }
            finally
            {
                this.Dispose();
            }
        }

        public void Dispose()
        {
            this._client.Disconnected -= OnClientDisconnected;
        }
    }
}
