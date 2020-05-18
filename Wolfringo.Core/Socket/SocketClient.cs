using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace TehGM.Wolfringo.Socket
{
    public class SocketClient : ISocketClient, IDisposable
    {
        private readonly ClientWebSocket _websocketClient;
        private CancellationTokenSource _connectionCts;

        public async Task ConnectAsync(Uri url, CancellationToken cancellationToken = default)
        {
            _connectionCts?.Cancel();
            _connectionCts?.Dispose();
            _connectionCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            await _websocketClient.ConnectAsync(url, _connectionCts.Token).ConfigureAwait(false);
            _ = ConnectionTaskAsync();
        }

        public Task DisconnectAsync(CancellationToken cancellationToken = default)
        {
            _connectionCts?.Cancel();
            _connectionCts?.Dispose();
            return _websocketClient.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by client", cancellationToken);
        }

        private async Task ConnectionTaskAsync(CancellationToken cancellationToken = default)
        {
            ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[1024 * 16]);

            while (!cancellationToken.IsCancellationRequested)
            {
                // TODO: implement message reading
            }
        }

        public void Dispose()
        {
            this.DisconnectAsync().GetAwaiter().GetResult();
        }
    }
}
