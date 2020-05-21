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

        public bool IsConnected => _websocketClient != null && (_websocketClient.State != WebSocketState.Closed || _websocketClient.State != WebSocketState.Aborted);

        private ClientWebSocket _websocketClient;
        private CancellationTokenSource _connectionCts;
        private readonly SemaphoreSlim _sendLock = new SemaphoreSlim(1);

        private static readonly SocketMessage _pingMessage = new SocketMessage(SocketMessageType.Ping, null, null);
        private static readonly ArraySegment<byte> _binaryPrepend = new ArraySegment<byte>(new byte[] { 4 });
        private uint _lastMessageID = 7;

        public event EventHandler Connected;
        public event EventHandler<SocketClosedEventArgs> Disconnected;
        public event EventHandler<SocketMessageEventArgs> MessageReceived;
        public event EventHandler<SocketMessageEventArgs> MessageSent;
        public event EventHandler<UnhandledExceptionEventArgs> ErrorRaised;

        public async Task ConnectAsync(Uri url, CancellationToken cancellationToken = default)
        {
            if (this.IsConnected)
                throw new InvalidOperationException("Already connected");

            this.Dispose();
            _connectionCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _websocketClient = new ClientWebSocket();
            await _websocketClient.ConnectAsync(url, _connectionCts.Token).ConfigureAwait(false);
            _ = ConnectionLoopAsync(_connectionCts.Token);
            Connected?.Invoke(this, EventArgs.Empty);
        }

        public async Task DisconnectAsync(CancellationToken cancellationToken = default)
        {
            if (!this.IsConnected)
                throw new InvalidOperationException("Not connected");

            await _websocketClient.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "Disconnection requested", cancellationToken);
            this.Dispose();
        }

        /// <summary>Sends message.</summary>
        /// <remarks>It is assumed that <paramref name="payload"/> already contains data placeholders if there's any <paramref name="binaryMessages"/>.</remarks>
        /// <param name="payload">Json payload for text message.</param>
        /// <param name="binaryMessages">Collection of binary messages to send.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        public Task<uint> SendAsync(JToken payload, IEnumerable<byte[]> binaryMessages, CancellationToken cancellationToken = default)
        {
            int binaryCount = binaryMessages?.Count() ?? 0;
            return SendInternalAsync(new SocketMessage(
                binaryCount == 0 ? SocketMessageType.Event : SocketMessageType.BinaryEvent, 
                ++_lastMessageID, 
                payload, 
                binaryCount),
                binaryMessages, cancellationToken);
        }

        private async Task<uint> SendInternalAsync(SocketMessage message, IEnumerable<byte[]> binaryMessages, CancellationToken cancellationToken = default)
        {
            if (!this.IsConnected)
                throw new InvalidOperationException("Not connected");

            try
            {
                await _sendLock.WaitAsync(cancellationToken).ConfigureAwait(false);
                byte[] data = Encoding.UTF8.GetBytes(message.ToString());
                using (CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _connectionCts.Token))
                {
                    // send the text message
                    await _websocketClient.SendAsync(new ArraySegment<byte>(data), WebSocketMessageType.Text, true, cts.Token).ConfigureAwait(false);

                    // send binary messages if any
                    if (message.BinaryMessagesCount != 0)
                    {
                        foreach (byte[] binMsg in binaryMessages)
                        {
                            await _websocketClient.SendAsync(_binaryPrepend, WebSocketMessageType.Binary, false, cts.Token).ConfigureAwait(false);
                            await _websocketClient.SendAsync(new ArraySegment<byte>(binMsg), WebSocketMessageType.Binary, true, cts.Token).ConfigureAwait(false);
                        }
                    }
                }
                MessageSent?.Invoke(this, new SocketMessageEventArgs(message, binaryMessages));
                return message.ID ?? default;
            }
            finally
            {
                _sendLock.Release();
            }
        }

        private async Task ConnectionLoopAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[1024 * 16]);

                while (!cancellationToken.IsCancellationRequested)
                {
                    if (_websocketClient == null || _websocketClient.State == WebSocketState.Closed)
                        _connectionCts?.Cancel();
                    else if (_websocketClient?.State == WebSocketState.CloseReceived)
                    {
                        await _websocketClient.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "Disconnection requested by server", cancellationToken).ConfigureAwait(false);
                        _connectionCts?.Cancel();
                    }
                    cancellationToken.ThrowIfCancellationRequested();

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
                        throw new InvalidDataException("Received a binary message while a text message was expected");
                }
            }
            catch (TaskCanceledException) { }
            catch (Exception ex)
            {
                ErrorRaised?.Invoke(this, new UnhandledExceptionEventArgs(ex, true));
                throw;
            }
            finally
            {
                Disconnected?.Invoke(this, new SocketClosedEventArgs(_websocketClient.CloseStatus.Value, _websocketClient.CloseStatusDescription));
                this.Dispose();
            }
        }

        private void OnTextMessageReceived(SocketMessage msg, IEnumerable<byte[]> binaryMessages)
        {
            if (msg.Type == SocketMessageType.SID)
            {
                this.Session = msg.Payload.ToObject<SocketSession>();
                _ = PingLoopAsync(this.Session, _connectionCts.Token);
            }
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
                if (result.CloseStatus != WebSocketCloseStatus.NormalClosure)
                    throw new WebSocketException(WebSocketError.ConnectionClosedPrematurely, result.CloseStatusDescription);
                return null;
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
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(session.PingInterval, cancellationToken).ConfigureAwait(false);
                    await SendInternalAsync(_pingMessage, null, cancellationToken).ConfigureAwait(false);
                }
            }
            catch (TaskCanceledException) { }
            catch (Exception ex)
            {
                ErrorRaised?.Invoke(this, new UnhandledExceptionEventArgs(ex, true));
                throw;
            }
        }

        public void Dispose()
        {
            this._websocketClient?.Dispose();
            this._websocketClient = null;
            if (this._connectionCts?.IsCancellationRequested != true)
                this._connectionCts?.Cancel();
            this._connectionCts?.Dispose();
            this._connectionCts = null;
        }
    }
}
