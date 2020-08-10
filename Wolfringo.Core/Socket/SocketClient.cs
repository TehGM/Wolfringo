﻿using Newtonsoft.Json.Linq;
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
    /// <summary>Basic SocketIO client designed for use with Wolfringo.</summary>
    /// <remarks><para>This client only supports features required to use with Wolf. For this reason, some SocketIO behaviours may not be supported, while others function in a Wolf-specific way.<br/>
    /// If you wish to support other behaviours, write own ISocketClient.</para>
    /// <para>This socket client will automatically handle pinging and SID message. It is not needed to handle these behaviours manually.</para></remarks>
    public class SocketClient : ISocketClient, IDisposable
    {
        /// <summary>Session info for the connection.</summary>
        public SocketSession Session { get; private set; }
        /// <summary>Encoding to use when sending and receiving messages. Defaults to UTF8.</summary>
        public Encoding MessageEncoding { get; set; } = Encoding.UTF8;
        /// <inheritdoc/>
        public bool IsConnected => _connectionCts != null && !_connectionCts.IsCancellationRequested &&
            (_websocketClient?.State == WebSocketState.Open || _websocketClient?.State == WebSocketState.Connecting);

        private ClientWebSocket _websocketClient;
        private CancellationTokenSource _connectionCts;
        private readonly SemaphoreSlim _sendLock = new SemaphoreSlim(1);

        private static readonly SocketMessage _pingMessage = new SocketMessage(SocketMessageType.Ping, null, null);
        private static readonly ArraySegment<byte> _binaryPrepend = new ArraySegment<byte>(new byte[] { 4 });
        private uint _lastMessageID = 7;

        /// <inheritdoc/>
        public event EventHandler Connected;
        /// <inheritdoc/>
        public event EventHandler<SocketClosedEventArgs> Disconnected;
        /// <inheritdoc/>
        public event EventHandler<SocketMessageEventArgs> MessageReceived;
        /// <inheritdoc/>
        public event EventHandler<SocketMessageEventArgs> MessageSent;
        /// <inheritdoc/>
        public event EventHandler<UnhandledExceptionEventArgs> ErrorRaised;

        /// <inheritdoc/>
        public async Task ConnectAsync(Uri url, CancellationToken cancellationToken = default)
        {
            if (this.IsConnected)
                throw new InvalidOperationException("Already connected");

            this.Clear();
            _websocketClient = new ClientWebSocket();
            await _websocketClient.ConnectAsync(url, cancellationToken).ConfigureAwait(false);
            Connected?.Invoke(this, EventArgs.Empty);
            _ = ConnectionLoopAsync();
        }

        /// <inheritdoc/>
        public Task DisconnectAsync(CancellationToken cancellationToken = default)
        {
            if (!this.IsConnected)
                throw new InvalidOperationException("Not connected");

            return _websocketClient.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "Disconnection requested", cancellationToken);
        }

        /// <inheritdoc/>
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

            using (CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _connectionCts.Token))
            {
                await _sendLock.WaitAsync(cts.Token).ConfigureAwait(false);
                try
                {
                    byte[] data = this.MessageEncoding.GetBytes(message.ToString());
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
                finally
                {
                    _sendLock.Release();
                }
            }

            MessageSent?.Invoke(this, new SocketMessageEventArgs(message, binaryMessages));
            return message.ID ?? default;
        }

        private async Task ConnectionLoopAsync()
        {
            Exception closeException = null;
            try
            {
                _lastMessageID = 7;
                _connectionCts = new CancellationTokenSource();
                ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[1024]);

                while (_connectionCts?.Token.IsCancellationRequested != true)
                {
                    // read from stream
                    SocketReceiveResult receivedMessage = await ReceiveAsync(buffer, _connectionCts.Token).ConfigureAwait(false);
                    if (!IsAnythingReceived(receivedMessage))
                        continue;

                    // parse the message
                    if (receivedMessage.MessageType == WebSocketMessageType.Text)
                    {
                        SocketMessage msg = SocketMessage.Parse(this.MessageEncoding.GetString(receivedMessage.ContentBytes));

                        // if message is binary, read them from stream as well
                        List<byte[]> binaryMessages = new List<byte[]>(msg.BinaryMessagesCount);
                        for (int i = 0; i < msg.BinaryMessagesCount; i++)
                        {
                            SocketReceiveResult receivedBinaryMessage = await ReceiveAsync(buffer, _connectionCts.Token).ConfigureAwait(false);
                            if (!IsAnythingReceived(receivedBinaryMessage))
                                continue;
                            if (receivedBinaryMessage.MessageType == WebSocketMessageType.Text)
                                throw new InvalidDataException("Received a text message while a binary message was expected");
                            binaryMessages.Add(receivedBinaryMessage.ContentBytes);
                        }
                        // raise event
                        OnTextMessageReceived(msg, binaryMessages);
                    }
                    else
                        throw new InvalidDataException("Received a binary message while a text message was expected");
                }
            }
            catch (Exception ex)
            {
                closeException = ex;
                // ignore premature close and task cancellations
                // these should be treated as normal close event, not an error
                if (!IsClosedPrematurelyException(ex) && !(ex is OperationCanceledException))
                    ErrorRaised?.Invoke(this, new UnhandledExceptionEventArgs(ex, true));
            }
            finally
            {
                // craft closed event, keeping the exception in mind
                WebSocketCloseStatus status = _websocketClient?.CloseStatus ??                              // websocketclient reported status has priority
                    (IsClosedPrematurelyException(closeException) ? WebSocketCloseStatus.ProtocolError :    // if premature close, report as protocol error
                    (closeException is OperationCanceledException) ? WebSocketCloseStatus.NormalClosure :   // if operation canceled, report as normal closure
                    WebSocketCloseStatus.Empty);                                                            // otherwise report unknown status
                string message = _websocketClient?.CloseStatusDescription ?? closeException?.Message;

                // clear cancels and nulls cts, so IsConnected will start returning false
                this.Clear();
                Disconnected?.Invoke(this, new SocketClosedEventArgs(status, message, closeException));
            }
        }

        private async Task CheckSocketStateAsync(CancellationToken cancellationToken = default)
        {
            // if closing was initiated by the server, acknowledge it before cancelling
            if (_websocketClient?.State == WebSocketState.CloseReceived)
            {
                await _websocketClient.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, 
                    "Disconnection requested by server", cancellationToken).ConfigureAwait(false);
                _connectionCts?.Cancel();
            }
            // trigger cancellation if client has been disposed or is in closed state
            else if (!this.IsConnected)
                _connectionCts?.Cancel();
            cancellationToken.ThrowIfCancellationRequested();
        }

        private void OnTextMessageReceived(SocketMessage msg, IEnumerable<byte[]> binaryMessages)
        {
            // handle SID message
            if (msg.Type == SocketMessageType.SID)
            {
                // store session
                this.Session = msg.Payload.ToObject<SocketSession>();
                // begin pinging loop
                _ = PingLoopAsync(this.Session, _connectionCts.Token);
            }
            // raise the event to listeners
            MessageReceived?.Invoke(this, new SocketMessageEventArgs(msg, binaryMessages ?? Enumerable.Empty<byte[]>()));
        }

        private static bool IsAnythingReceived(SocketReceiveResult result)
            => result != null && result.ContentBytes != null && result.ContentBytes.Length != 0;

        private async Task<SocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken = default)
        {
            await CheckSocketStateAsync(cancellationToken).ConfigureAwait(false);

            // read stream
            WebSocketReceiveResult result = await _websocketClient.ReceiveAsync(buffer, cancellationToken).ConfigureAwait(false);
            // cancel further execution if connection was closed
            if (result?.MessageType == WebSocketMessageType.Close || !this.IsConnected)
                return null;

            byte[] contents = null;
            int bytesRead = 0;
            if (!result.EndOfMessage)
            {
                // borrowed from Discord.NET's handling of websockets
                using (MemoryStream stream = new MemoryStream())
                {
                    stream.Write(buffer.Array, buffer.Offset, result.Count);
                    do
                    {
                        await CheckSocketStateAsync(cancellationToken).ConfigureAwait(false);
                        result = await _websocketClient.ReceiveAsync(buffer, cancellationToken).ConfigureAwait(false);
                        // cancel further execution if connection was closed
                        if (result?.MessageType == WebSocketMessageType.Close || !this.IsConnected)
                            return null;
                        stream.Write(buffer.Array, buffer.Offset, result.Count);
                    }
                    while (result == null || !result.EndOfMessage);

                    bytesRead = (int)stream.Length;
                    if (stream.TryGetBuffer(out ArraySegment<byte> streamBuffer))
                        contents = streamBuffer.Array;
                    else contents = streamBuffer.ToArray();
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
                while (!cancellationToken.IsCancellationRequested && this.IsConnected)
                {
                    await Task.Delay(session.PingInterval, cancellationToken).ConfigureAwait(false);
                    if (!cancellationToken.IsCancellationRequested && this.IsConnected)
                        await SendInternalAsync(_pingMessage, null, cancellationToken).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                ErrorRaised?.Invoke(this, new UnhandledExceptionEventArgs(ex, true));
            }
        }

        private void Clear()
        {
            try { this._connectionCts?.Cancel(); } catch { }
            try { this._connectionCts?.Dispose(); } catch { }
            this._connectionCts = null;
            this.Session = null;
            try { this._websocketClient?.Dispose(); } catch { }
            this._websocketClient = null;
        }

        private static bool IsClosedPrematurelyException(Exception ex)
            => ex != null && ex is WebSocketException wsEx && wsEx.WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely;

        /// <summary>Disposes the resources used by this client.</summary>
        public void Dispose()
        {
            this.Clear();
            this._sendLock?.Dispose();
        }
    }
}
