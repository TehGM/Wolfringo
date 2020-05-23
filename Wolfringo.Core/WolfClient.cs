using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TehGM.Wolfringo.Messages;
using TehGM.Wolfringo.Messages.Responses;
using TehGM.Wolfringo.Messages.Serialization;
using TehGM.Wolfringo.Socket;
using TehGM.Wolfringo.Utilities;
using TehGM.Wolfringo.Utilities.Internal;

namespace TehGM.Wolfringo
{
    public class WolfClient : IWolfClient, IDisposable
    {
        public string Url { get; }
        public string Token { get; }
        public string Device { get; }
        public WolfUser CurrentUser { get; private set; }

        public event EventHandler Connected;
        public event EventHandler Disconnected;
        public event EventHandler<WolfMessageEventArgs> MessageReceived;
        public event EventHandler<WolfMessageSentEventArgs> MessageSent;
        public event EventHandler<UnhandledExceptionEventArgs> ErrorRaised;

        private readonly ISocketClient _client;
        private readonly ISerializerMap<string, IMessageSerializer> _messageSerializers;
        private readonly ISerializerMap<Type, IResponseSerializer> _responseSerializers;
        private readonly IResponseTypeResolver _responseTypeResolver;
        private readonly ILogger _log;
        private readonly MessageCallbackDispatcher _callbackDispatcher;

        #region Constructors
        public WolfClient(string url, string device, string token, ILogger logger = null, 
            ISerializerMap<string, IMessageSerializer> messageSerializers = null, ISerializerMap<Type, IResponseSerializer> responseSerializers = null, IResponseTypeResolver responseTypeResolver = null)
        {
            // verify input
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException(nameof(url));
            if (string.IsNullOrWhiteSpace(device))
                throw new ArgumentNullException(nameof(device));
            if (token != null && string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Token can be null for auto-generation or have a value, but it cannot be empty or whitespace", nameof(token));

            // set provided props
            this.Url = url;
            this.Device = device;
            this.Token = token;
            this._log = logger;
            this._responseTypeResolver = responseTypeResolver ?? new DefaultResponseTypeResolver();
            this._messageSerializers = messageSerializers ?? new DefaultMessageSerializerMap();
            this._responseSerializers = responseSerializers ?? new DefaultResponseSerializerMap();

            // init dispatcher
            _callbackDispatcher = new MessageCallbackDispatcher();

            // init socket client
            this._client = new SocketClient();
            this._client.MessageReceived += OnClientMessageReceived;
            this._client.MessageSent += OnClientMessageSent;
            this._client.Connected += OnClientConnected;
            this._client.Disconnected += OnClientDisconnected;
            this._client.ErrorRaised += OnClientError;
        }

        public WolfClient(string url, string device, ILogger logger = null, 
            ITokenProvider tokenProvider = null, 
            ISerializerMap<string, IMessageSerializer> messageSerializers = null, ISerializerMap<Type, IResponseSerializer> responseSerializers = null, IResponseTypeResolver responseTypeResolver = null)
            : this(url, device, GetNewToken(tokenProvider), logger, messageSerializers, responseSerializers, responseTypeResolver) { }

        public WolfClient(WolfClientOptions options, ILogger logger = null, 
            ITokenProvider tokenProvider = null,
            ISerializerMap<string, IMessageSerializer> messageSerializers = null, ISerializerMap<Type, IResponseSerializer> responseSerializers = null, IResponseTypeResolver responseTypeResolver = null)
            : this(options.ServerURL, options.Device, options.Token ?? GetNewToken(tokenProvider), logger, messageSerializers, responseSerializers, responseTypeResolver) { }

        public WolfClient(ILogger logger = null, 
            ITokenProvider tokenProvider = null,
            ISerializerMap<string, IMessageSerializer> messageSerializers = null, ISerializerMap<Type, IResponseSerializer> responseSerializers = null, IResponseTypeResolver responseTypeResolver = null)
            : this(WolfClientOptions.DefaultServerURL, WolfClientOptions.DefaultDevice, logger, tokenProvider, messageSerializers, responseSerializers, responseTypeResolver) { }

        private static string GetNewToken(ITokenProvider tokenProvider = null)
        {
            if (tokenProvider == null)
                tokenProvider = new DefaultWolfTokenProvider();
            return tokenProvider.GenerateToken(18);
        }
        #endregion

        #region Connection management
        public Task ConnectAsync(CancellationToken cancellationToken = default)
            => _client.ConnectAsync(
                new Uri(new Uri(this.Url), $"/socket.io/?token={this.Token}&device={this.Device}&EIO=3&transport=websocket"),
                cancellationToken);

        public Task DisconnectAsync(CancellationToken cancellationToken = default)
            => _client.DisconnectAsync(cancellationToken);

        public void Dispose()
            => (_client as IDisposable)?.Dispose();
        #endregion

        #region Message Sending
        public async Task<TResponse> SendAsync<TResponse>(IWolfMessage message, CancellationToken cancellationToken = default) where TResponse : WolfResponse
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (string.IsNullOrWhiteSpace(message.Command))
                throw new ArgumentException("Message command cannot be null, empty or whitespace", nameof(message));

            if (!_messageSerializers.TryFindMappedSerializer(message.Command, out IMessageSerializer serializer))
            {
                // try fallback simple serialization
                _log?.LogWarning("Serializer for command {Command} not found, using fallback one", message.Command);
                serializer = _messageSerializers.FallbackSerializer;
            }
            SerializedMessageData data = serializer.Serialize(message);

            uint msgId = await _client.SendAsync(message.Command, data.Payload, data.BinaryMessages, cancellationToken).ConfigureAwait(false);
            WolfResponse response = await AwaitResponseAsync<TResponse>(msgId, message, cancellationToken).ConfigureAwait(false);
            if (response.IsError)
                throw new MessageSendingException(response);
            this.MessageSent?.Invoke(this, new WolfMessageSentEventArgs(message, response));
            return response as TResponse;
        }

        private Task<WolfResponse> AwaitResponseAsync<TResponse>(uint messageId, IWolfMessage sentMessage, 
            CancellationToken cancellationToken = default) where TResponse : WolfResponse
        {
            TaskCompletionSource<WolfResponse> tcs = new TaskCompletionSource<WolfResponse>();
            EventHandler<SocketMessageEventArgs> callback = null;
            CancellationTokenRegistration ctr = cancellationToken.Register(() => tcs.TrySetCanceled(cancellationToken));
            callback = (sender, e) =>
            {
                try
                {
                    // ignore messages that are no acks
                    if (e.Message.Type != SocketMessageType.EventAck && e.Message.Type != SocketMessageType.BinaryEventAck)
                        return;
                    // only accept response with corresponding message ID
                    if (e.Message.ID == null)
                        return;
                    if (e.Message.ID.Value != messageId)
                        return;

                    // parse response
                    Type responseType = _responseTypeResolver?.GetMessageResponseType<TResponse>(sentMessage) ?? typeof(TResponse);
                    if (!_responseSerializers.TryFindMappedSerializer(responseType, out IResponseSerializer serializer))
                    {
                        _log?.LogWarning("Serializer for response type {Type} not found, using fallback one", responseType.FullName);
                        serializer = _responseSerializers.FallbackSerializer;
                    }
                    WolfResponse response = serializer.Deserialize(responseType, new SerializedMessageData(e.Message.Payload, e.BinaryMessages));

                    // if it's a login message, we can also extract current user
                    if (response is LoginResponse loginResponse)
                        this.CurrentUser = WolfUser.FromLoginResponse(loginResponse);

                    // set task result to finish it, and unhook the event to prevent memory leaks
                    tcs.TrySetResult(response);
                    ctr.Dispose();
                    if (_client != null)
                        _client.MessageReceived -= callback;
                }
                catch (Exception ex)
                {
                    // don't rethrow exception here, as doing so will kill the socket client loop
                    _log?.LogError(ex, "Exception has occured when handling socket response");
                    tcs.TrySetException(ex);
                }
            };
            _client.MessageReceived += callback;
            return tcs.Task;
        }
        #endregion

        #region Event handlers
        private void OnClientMessageReceived(object sender, SocketMessageEventArgs e)
        {
            try
            {
                TryLogMessageTrace(e, "Received");

                if (TryParseCommandEvent(e.Message, out string command, out JToken payload))
                {
                    // find serializer for command
                    if (!_messageSerializers.TryFindMappedSerializer(command, out IMessageSerializer serializer))
                    {
                        // don't throw exception here, as doing so will kill the socket client loop
                        _log?.LogError("Serializer for command {Command} not found", command);
                        return;
                    }
                    // deserialize message
                    IWolfMessage msg = serializer.Deserialize(command, new SerializedMessageData(payload, e.BinaryMessages));
                    if (msg == null)
                        return;
                    // ignore own messages
                    if (msg is ChatMessage chatMessage && this.CurrentUser != null && chatMessage.SenderID.Value == this.CurrentUser.ID)
                        return;

                    _log?.LogDebug("Message received: {Command}", command);
                    this.MessageReceived?.Invoke(this, new WolfMessageEventArgs(msg));
                    _callbackDispatcher.Invoke(msg);
                }
            }
            catch (Exception ex)
            {
                // don't rethrow exception here, as doing so will kill the socket client loop
                _log?.LogError(ex, "Exception occured when handling received message");
                this.ErrorRaised?.Invoke(this, new UnhandledExceptionEventArgs(ex, false));
            }
        }

        public void AddMessageListener<T>(Action<T> listener) where T : IWolfMessage
            => _callbackDispatcher.Add(new TypedMessageCallback<T>(listener));

        public void RemoveMessageListener<T>(Action<T> listener) where T : IWolfMessage
            => _callbackDispatcher.Remove(new TypedMessageCallback<T>(listener));

        private void OnClientMessageSent(object sender, SocketMessageEventArgs e)
        {
            TryLogMessageTrace(e, "Sent");
            if (TryParseCommandEvent(e.Message, out string command, out _))
                _log?.LogDebug("Message sent: {Command}", command);
        }

        private void OnClientConnected(object sender, EventArgs e)
        {
            _log?.LogInformation("Connected to {URL} as {Device}", this.Url, this.Device);
            this.Connected?.Invoke(this, EventArgs.Empty);
        }

        private void OnClientDisconnected(object sender, SocketClosedEventArgs e)
        {
            if (e.CloseStatus == System.Net.WebSockets.WebSocketCloseStatus.NormalClosure)
                _log?.LogInformation("Disconnected ({Description})", e.CloseStatusDescription);
            else
                _log?.LogWarning("Disconnected ungracefully ({Status}, {Description})", e.CloseStatus.ToString(), e.CloseStatusDescription);
            this.Disconnected?.Invoke(this, EventArgs.Empty);
        }

        private void OnClientError(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
                _log?.LogError(ex, "Socket client error: {Error}", ex.Message);
            else _log?.LogError("Socket client error: {Error}", e.ExceptionObject?.ToString());
            this.ErrorRaised?.Invoke(this, e);
        }
        #endregion

        #region Internal helpers
        private bool TryParseCommandEvent(SocketMessage message, out string command, out JToken payload)
        {
            if ((message.Type == SocketMessageType.BinaryEvent || message.Type == SocketMessageType.Event)
                && message.Payload is JArray array)
            {
                command = array.First.ToObject<string>();
                payload = array.First.Next;
                return true;
            }
            else
            {
                command = null;
                payload = message.Payload;
                return false;
            }
        }

        private void TryLogMessageTrace(SocketMessageEventArgs e, string keyword)
        {
            if (_log?.IsEnabled(LogLevel.Trace) == true)
            {
                if (e.BinaryMessages?.Any() == true)
                {
                    string binaryMessages = string.Join("\n", e.BinaryMessages.Where(msg => msg?.Any() == true).Select(msg => Encoding.UTF8.GetString(msg)));
                    _log?.LogTrace($"{keyword} {{MessageType}}: {{Message}}\n{{BinaryMessages}}", e.Message.Type.ToString(), e.Message.ToString(), binaryMessages);
                }
                else
                    _log?.LogTrace($"{keyword} {{MessageType}}: {{Message}}", e.Message.Type.ToString(), e.Message.ToString());
            }
        }
        #endregion
    }
}
