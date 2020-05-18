using System;
using System.IO;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace TehGM.Wolfringo.Socket
{
    public class SocketClient : ISocketClient, IDisposable
    {
        private readonly ClientWebSocket _websocketClient;
        private CancellationTokenSource _connectionCts;

        public event EventHandler<SocketClosedEventArgs> Disconnected;

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
            return _websocketClient.CloseAsync(WebSocketCloseStatus.NormalClosure, $"Disconnection requested", cancellationToken);
        }

        private async Task ConnectionTaskAsync(CancellationToken cancellationToken = default)
        {
            ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[1024 * 16]);

            while (!cancellationToken.IsCancellationRequested)
            {
                SocketReceiveResult receivedMessage = await ReceiveAsync(buffer, cancellationToken).ConfigureAwait(false);
                if (receivedMessage == null || receivedMessage.BytesRead == 0)
                    continue;

                // TODO: parse message
            }
        }

        private async Task<SocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken = default)
        {
            WebSocketReceiveResult result = await _websocketClient.ReceiveAsync(buffer, cancellationToken).ConfigureAwait(false);
            // cancel further execution if connection was closed
            if (result.MessageType == WebSocketMessageType.Close)
            {
                _connectionCts?.Cancel();
                Disconnected?.Invoke(this, new SocketClosedEventArgs(result.CloseStatus.Value, result.CloseStatusDescription));
                if (result.CloseStatus != WebSocketCloseStatus.NormalClosure)
                    throw new WebSocketException(WebSocketError.ConnectionClosedPrematurely, result.CloseStatusDescription);
                else return null;
            }

            byte[] contents = null;
            int bytesRead = 0;
            if (!result.EndOfMessage)
            {
                // borrowed from Discord.NET's handling of websockets
                using (MemoryStream stream = new MemoryStream())
                {
                    stream.Write(buffer.Array, 0, result.Count);
                    do
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        result = await _websocketClient.ReceiveAsync(buffer, cancellationToken).ConfigureAwait(false);
                        stream.Write(buffer.Array, 0, result.Count);
                    }
                    while (result == null || !result.EndOfMessage);

                    bytesRead = (int)stream.Length;
                    if (stream.TryGetBuffer(out buffer))
                        contents = buffer.Array;
                    else contents = stream.ToArray();
                }
            }
            else
            {
                bytesRead = result.Count;
                contents = buffer.Array;
            }

            return new SocketReceiveResult(contents, bytesRead, result.MessageType);
        }

        public void Dispose()
        {
            this.DisconnectAsync().GetAwaiter().GetResult();
        }
    }
}
