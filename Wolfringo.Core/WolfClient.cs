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
using TehGM.Wolfringo.Utilities.Internal;

namespace TehGM.Wolfringo
{
    public class WolfClient : IWolfClient, IDisposable
    {
        public string Url { get; }
        public string Token { get; }
        public string Device { get; }

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
        private readonly IWolfEntityCache<WolfUser> _usersCache;
        private readonly IWolfEntityCache<WolfGroup> _groupsCache;

        private CancellationTokenSource _cts;
        private uint _currentUserID;

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

            // init caches
            _usersCache = new WolfEntityCache<WolfUser>();
            _groupsCache = new WolfEntityCache<WolfGroup>();

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
        {
            if (this._client.IsConnected)
                throw new InvalidOperationException("Already connected");

            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            return _client.ConnectAsync(
                new Uri(new Uri(this.Url), $"/socket.io/?token={this.Token}&device={this.Device.ToLower()}&EIO=3&transport=websocket"),
                _cts.Token);
        }

        public Task DisconnectAsync(CancellationToken cancellationToken = default)
            => _client.DisconnectAsync(cancellationToken);

        public void Dispose()
            => (_client as IDisposable)?.Dispose();

        private void Clear()
        {
            this._cts?.Cancel();
            this._cts?.Dispose();
            this._currentUserID = default;
            this._usersCache?.Clear();
        }
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
            IWolfResponse response = await AwaitResponseAsync<TResponse>(msgId, message, cancellationToken).ConfigureAwait(false);
            if (response.IsError())
                throw new MessageSendingException(response);
            this.MessageSent?.Invoke(this, new WolfMessageSentEventArgs(message, response));
            return response as TResponse;
        }

        private Task<IWolfResponse> AwaitResponseAsync<TResponse>(uint messageId, IWolfMessage sentMessage, 
            CancellationToken cancellationToken = default) where TResponse : IWolfResponse
        {
            TaskCompletionSource<IWolfResponse> tcs = new TaskCompletionSource<IWolfResponse>();
            EventHandler<SocketMessageEventArgs> callback = null;
            CancellationTokenRegistration ctr = cancellationToken.Register(() => tcs.TrySetCanceled(cancellationToken));
            callback = async (sender, e) =>
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
                    SerializedMessageData responseData = new SerializedMessageData(e.Message.Payload, e.BinaryMessages);
                    IWolfResponse response = serializer.Deserialize(responseType, responseData);
                    await OnMessageSentInternalAsync(sentMessage, response, responseData, cancellationToken).ConfigureAwait(false);

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

        /// <summary>Internal method for handling additional actions on sent message.</summary>
        /// <param name="message">Sent message.</param>
        /// <param name="response">Response received.</param>
        /// <param name="cancellationToken"></param>
        private Task OnMessageSentInternalAsync(IWolfMessage message, IWolfResponse response, SerializedMessageData rawResponse, CancellationToken cancellationToken = default)
        {
            // if it's a login message, we can extract current user ID
            if (response is LoginResponse loginResponse)
                this._currentUserID = loginResponse.UserID;
            // if it's chat message, populate with response info to get timestamp
            if (message is ChatMessage chatMsg && response is ChatResponse)
                rawResponse?.Payload?.First?.PopulateObject(ref chatMsg, "body");
            // update users cache if it's user profile message
            if (response is UserProfileResponse userProfileResponse && userProfileResponse.UserProfiles?.Any() == true)
            {
                foreach (WolfUser user in userProfileResponse.UserProfiles)
                    _usersCache?.AddOrReplaceIfChanged(user);
            }
            if (response is GroupProfileResponse groupProfileResponse && groupProfileResponse.GroupProfiles?.Any() == true)
            {
                foreach (WolfGroup group in groupProfileResponse.GroupProfiles)
                    _groupsCache?.AddOrReplaceIfChanged(group);
            }
            // TODO: handle other types
            return Task.CompletedTask;
        }
        #endregion

        #region Caching
        public async Task<IEnumerable<WolfUser>> GetUsersAsync(IEnumerable<uint> userIDs, CancellationToken cancellationToken = default)
        {
            if (!this._client.IsConnected)
                throw new InvalidOperationException("Not connected");
            if (userIDs?.Any() != true)
                throw new ArgumentException("There must be at least one user ID to retrieve", nameof(userIDs));

            // get as many users from cache as possible
            List<WolfUser> results = new List<WolfUser>(userIDs.Count());
            foreach (uint uID in userIDs)
            {
                WolfUser cachedUser = _usersCache?.Get(uID);
                if (cachedUser != null)
                {
                    _log?.LogTrace("User {UserID} found in cache", uID);
                    results.Add(cachedUser);
                }
            }

            // get the ones that aren't in cache from the server
            IEnumerable<uint> toRequest = userIDs.Except(results.Select(u => u.ID));
            if (toRequest.Any())
            {
                UserProfileResponse response = await SendAsync<UserProfileResponse>(
                    new UserProfileMessage(toRequest, true, true), cancellationToken).ConfigureAwait(false);
                results.AddRange(response.UserProfiles);
            }

            // return results
            if (!results.Any())
                throw new KeyNotFoundException();
            return results;
        }

        public Task<WolfUser> GetCurrentUserAsync(CancellationToken cancellationToken = default)
        {
            if (this._currentUserID == default)
                throw new InvalidOperationException("Not logged in");
            return this.GetUserAsync(this._currentUserID, cancellationToken);
        }

        public async Task<IEnumerable<WolfGroup>> GetGroupsAsync(IEnumerable<uint> groupIDs, CancellationToken cancellationToken = default)
        {
            if (!this._client.IsConnected)
                throw new InvalidOperationException("Not connected");
            if (groupIDs?.Any() != true)
                throw new ArgumentException("There must be at least one group ID to retrieve", nameof(groupIDs));


            // get as many users from cache as possible
            List<WolfGroup> results = new List<WolfGroup>(groupIDs.Count());
            foreach (uint gID in groupIDs)
            {
                WolfGroup cachedGroup = _groupsCache?.Get(gID);
                if (cachedGroup != null)
                {
                    _log?.LogTrace("Group {GroupID} found in cache", gID);
                    results.Add(cachedGroup);
                }
            }

            // get the ones that aren't in cache from the server
            IEnumerable<uint> toRequest = groupIDs.Except(results.Select(u => u.ID));
            if (toRequest.Any())
            {
                GroupProfileResponse response = await SendAsync<GroupProfileResponse>(
                    new GroupProfileMessage(toRequest, true), cancellationToken).ConfigureAwait(false);
                results.AddRange(response.GroupProfiles);
            }

            // return results
            if (!results.Any())
                throw new KeyNotFoundException();
            return results;
        }
        #endregion

        #region Event handlers
        private async void OnClientMessageReceived(object sender, SocketMessageEventArgs e)
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
                    SerializedMessageData rawData = new SerializedMessageData(payload, e.BinaryMessages);
                    IWolfMessage msg = serializer.Deserialize(command, rawData);
                    if (msg == null)
                        return;
                    // ignore own messages
                    if (msg is ChatMessage chatMessage && chatMessage.SenderID.Value == this._currentUserID)
                        return;

                    _log?.LogDebug("Message received: {Command}", command);
                    await OnMessageReceivedInternalAsync(msg, rawData, _cts.Token).ConfigureAwait(false);
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

        private Task OnMessageReceivedInternalAsync(IWolfMessage message, SerializedMessageData rawMessage, CancellationToken cancellationToken = default)
        {
            if (message is PresenceUpdateMessage presenceUpdate)
            {
                WolfUser cachedUser = _usersCache?.Get(presenceUpdate.UserID);
                if (cachedUser != null)
                {
                    _log?.LogTrace("Updating cached user {UserID} presence", cachedUser.ID);
                    rawMessage.Payload.PopulateObject(ref cachedUser, "body");
                }
            }
            return Task.CompletedTask;
        }

        public void AddMessageListener(IMessageCallback listener)
            => _callbackDispatcher.Add(listener);
        public void RemoveMessageListener(IMessageCallback listener)
            => _callbackDispatcher.Remove(listener);

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
            this.Clear();
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
