using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TehGM.Wolfringo.Messages;
using TehGM.Wolfringo.Messages.Responses;
using TehGM.Wolfringo.Messages.Serialization;
using TehGM.Wolfringo.Messages.Serialization.Internal;
using TehGM.Wolfringo.Socket;
using TehGM.Wolfringo.Utilities;

namespace TehGM.Wolfringo
{
    public class WolfClient : IWolfClient, IDisposable
    {
        public string Url { get; }
        public string Token { get; }
        public string Device { get; }
        public WolfUser CurrentUser { get; private set; }

        public event Action<IWolfMessage> MessageReceived;

        private readonly ISocketClient _client;
        private readonly IDictionary<string, IMessageSerializer> _serializers;
        private readonly IMessageSerializer _fallbackSerializer;
        private readonly ILogger _log;

        #region Constructors
        public WolfClient(string url, string device, string token, ILogger logger = null)
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

            // init default serializers
            this._serializers = GetDefaultMessageSerializers();
            this._fallbackSerializer = new JsonMessageSerializer<IWolfMessage>();

            // init socket client
            this._client = new SocketClient();
            this._client.MessageReceived += OnClientMessageReceived;
            this._client.MessageSent += OnClientMessageSent;
            this._client.Connected += OnClientConnected;
            this._client.Disconnected += OnClientDisconnected;
            this._client.ErrorRaised += OnClientError;
        }

        public WolfClient(string url, string device, ILogger logger = null, ITokenProvider tokenProvider = null)
            : this(url, device, GetNewToken(tokenProvider), logger) { }

        public WolfClient(WolfClientOptions options, ILogger logger = null, ITokenProvider tokenProvider = null)
            : this(options.ServerURL, options.Device, options.Token ?? GetNewToken(tokenProvider), logger) { }

        public WolfClient(ILogger logger = null, ITokenProvider tokenProvider = null)
            : this(WolfClientOptions.DefaultServerURL, WolfClientOptions.DefaultDevice, logger, tokenProvider) { }

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

            SerializedMessageData data;
            if (_serializers.TryGetValue(message.Command, out IMessageSerializer serializer))
                data = serializer.Serialize(message);
            else
            {
                // try fallback simple serialization
                _log?.LogWarning("Serializer for command {Command} not found, using fallback one", message.Command);
                data = _fallbackSerializer.Serialize(message);
            }

            uint msgId = await _client.SendAsync(message.Command, data.Payload, data.BinaryMessages, cancellationToken).ConfigureAwait(false);
            return await AwaitResponseAsync<TResponse>(msgId, cancellationToken).ConfigureAwait(false);
        }

        private Task<TResponse> AwaitResponseAsync<TResponse>(uint messageId, CancellationToken cancellationToken = default) where TResponse : WolfResponse
        {
            TaskCompletionSource<TResponse> tcs = new TaskCompletionSource<TResponse>();
            EventHandler<SocketMessageEventArgs> callback = null;
            CancellationTokenRegistration ctr = cancellationToken.Register(() => tcs.TrySetCanceled(cancellationToken));
            callback = (sender, e) =>
            {
                try
                {
                    // only accept response with corresponding message ID
                    if (e.Message.ID == null)
                        return;
                    if (e.Message.ID.Value != messageId)
                        return;

                    // parse response
                    JToken responseObject = (e.Message.Payload is JArray) ? e.Message.Payload.First : e.Message.Payload;
                    TResponse response = ParseResponse<TResponse>(responseObject);

                    // if it's a login message, we can also extract current user
                    if (response is LoginResponse loginResponse && loginResponse.Username != default)
                        this.CurrentUser = ParseResponse<WolfUser>(responseObject);

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

            T ParseResponse<T>(JToken response)
            {
                T result = response.ToObject<T>(SerializationHelper.DefaultSerializer);
                // if response has body or headers, further use it to populate the response entity
                response.PopulateObject(ref result, "headers");
                response.PopulateObject(ref result, "body");
                return result;
            }
        }

        public void SetSerializer(string command, IMessageSerializer serializer)
            => this._serializers[command] = serializer;
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
                    if (!_serializers.TryGetValue(command, out IMessageSerializer serializer))
                    {
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
                    this.MessageReceived?.Invoke(msg);
                }
            }
            catch (Exception ex)
            {
                // don't rethrow exception here, as doing so will kill the socket client loop
                _log?.LogError(ex, "Exception occured when handling received message");
            }
        }

        private void OnClientMessageSent(object sender, SocketMessageEventArgs e)
        {
            TryLogMessageTrace(e, "Sent");
            if (TryParseCommandEvent(e.Message, out string command, out _))
                _log?.LogDebug("Message sent: {Command}", command);
        }

        private void OnClientConnected(object sender, EventArgs e)
        {
            _log?.LogInformation("Connected to {URL} as {Device}", this.Url, this.Device);
        }

        private void OnClientDisconnected(object sender, SocketClosedEventArgs e)
        {
            if (e.CloseStatus == System.Net.WebSockets.WebSocketCloseStatus.NormalClosure)
                _log?.LogInformation("Disconnected ({Description})", e.CloseStatusDescription);
            else
                _log?.LogWarning("Disconnected ungracefully ({Status}, {Description})", e.CloseStatus.ToString(), e.CloseStatusDescription);
        }

        private void OnClientError(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
                _log?.LogError(ex, "Socket client error: {Error}", ex.Message);
            else _log?.LogError("Socket client error: {Error}", e.ExceptionObject?.ToString());
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

        protected virtual IDictionary<string, IMessageSerializer> GetDefaultMessageSerializers()
        {
            return new Dictionary<string, IMessageSerializer>(StringComparer.OrdinalIgnoreCase)
            {
                { MessageCommands.Welcome, new JsonMessageSerializer<WelcomeMessage>() },
                { MessageCommands.Login, new JsonMessageSerializer<LoginMessage>() },
                { MessageCommands.SubscribeToPm, new JsonMessageSerializer<SubscribeToPmMessage>() },
                { MessageCommands.SubscribeToGroup, new JsonMessageSerializer<SubscribeToGroupMessage>() },
                { MessageCommands.Chat, new ChatMessageSerializer() }
            };
        }
        #endregion
    }
}
