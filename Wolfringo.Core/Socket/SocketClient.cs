using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TehGM.Wolfringo.Socket
{
    public class SocketClient : ISocketClient, IDisposable
    {
        public SocketSession Session { get; private set; }
        public TimeSpan Latency { get; private set; }

        private readonly ClientWebSocket _websocketClient;
        private CancellationTokenSource _connectionCts;
        private readonly SemaphoreSlim _sendLock = new SemaphoreSlim(1);

        private static readonly SocketMessage _pingMessage = new SocketMessage(SocketMessageType.Ping, null, null);
        private uint _lastMessageID = 7;
        private DateTime _lastPingSentUtc;

        public event EventHandler<SocketClosedEventArgs> Disconnected;
        public event EventHandler<SocketMessageEventArgs> MessageReceived;
        public event EventHandler<SocketMessageEventArgs> MessageSent;

        public SocketClient()
        {
            this._websocketClient = new ClientWebSocket();
        }

        public async Task ConnectAsync(Uri url, CancellationToken cancellationToken = default)
        {
            _connectionCts?.Cancel();
            _connectionCts?.Dispose();
            _connectionCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            await _websocketClient.ConnectAsync(url, _connectionCts.Token).ConfigureAwait(false);
            _ = ConnectionLoopAsync();
        }

        public Task DisconnectAsync(CancellationToken cancellationToken = default)
        {
            _connectionCts?.Cancel();
            _connectionCts?.Dispose();
            return _websocketClient.CloseAsync(WebSocketCloseStatus.NormalClosure, $"Disconnection requested", cancellationToken);
        }

        public Task SendAsync(JToken payload, CancellationToken cancellationToken = default)
            => SendInternalAsync(new SocketMessage(SocketMessageType.Event, ++_lastMessageID, payload), cancellationToken);

        private async Task SendInternalAsync(SocketMessage message, CancellationToken cancellationToken = default)
        {
            try
            {
                await _sendLock.WaitAsync().ConfigureAwait(false);
                byte[] data = Encoding.UTF8.GetBytes(message.ToString());

                using (CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _connectionCts.Token))
                    await _websocketClient.SendAsync(new ArraySegment<byte>(data), WebSocketMessageType.Text, true, cancellationToken).ConfigureAwait(false);
                MessageSent?.Invoke(this, new SocketMessageEventArgs(message, Enumerable.Empty<byte[]>()));
            }
            finally
            {
                _sendLock.Release();
            }
        }

        private async Task ConnectionLoopAsync(CancellationToken cancellationToken = default)
        {
            ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[1024 * 16]);

            while (!cancellationToken.IsCancellationRequested)
            {
                SocketReceiveResult receivedMessage = await ReceiveAsync(buffer, cancellationToken).ConfigureAwait(false);
                if (!IsAnythingReceived(receivedMessage))
                    continue;

                if (receivedMessage.MessageType == WebSocketMessageType.Text)
                {
                    SocketMessage msg = SocketMessage.Parse(Encoding.UTF8.GetString(receivedMessage.ContentBytes));
                    List<byte[]> binaryMessages = new List<byte[]>(msg.BinaryMessagesCount);
                    for (int i = 0; i < msg.BinaryMessagesCount; i++)
                    {
                        SocketReceiveResult receivedBinaryMessage = await ReceiveAsync(buffer, cancellationToken).ConfigureAwait(false);
                        if (!IsAnythingReceived(receivedBinaryMessage))
                            continue;
                        if (receivedBinaryMessage.MessageType == WebSocketMessageType.Text)
                            throw new InvalidDataException("Received a text message while a binary message was expected");
                        binaryMessages.Add(receivedBinaryMessage.ContentBytes);
                    }
                    OnTextMessageReceived(msg, binaryMessages);
                }
                else
                {
                    throw new InvalidDataException("Received a binary message while a text message was expected");
                }
            }
        }

        private void OnTextMessageReceived(SocketMessage msg, IEnumerable<byte[]> binaryMessages)
        {
            if (msg.Type == SocketMessageType.SID)
            {
                this.Session = msg.Payload.ToObject<SocketSession>();
                _ = PingLoopAsync(this.Session, _connectionCts.Token);
            }
            else if (msg.Type == SocketMessageType.Pong)
                this.Latency = DateTime.UtcNow - _lastPingSentUtc;
            MessageReceived?.Invoke(this, new SocketMessageEventArgs(msg, binaryMessages ?? Enumerable.Empty<byte[]>()));
        }

        private static bool IsAnythingReceived(SocketReceiveResult result)
            => result != null && result.ContentBytes != null && result.ContentBytes.Length != 0;

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

        private async Task PingLoopAsync(SocketSession session, CancellationToken cancellationToken = default)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(session.PingInterval, cancellationToken).ConfigureAwait(false);
                await SendInternalAsync(_pingMessage, cancellationToken).ConfigureAwait(false);
                _lastPingSentUtc = DateTime.UtcNow;
            }
        }

        public void Dispose()
        {
            this.DisconnectAsync().GetAwaiter().GetResult();
        }
    }
}
