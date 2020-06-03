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
using TehGM.Wolfringo.Messages.Serialization.Internal;
using TehGM.Wolfringo.Socket;
using TehGM.Wolfringo.Utilities;
using TehGM.Wolfringo.Utilities.Internal;

namespace TehGM.Wolfringo
{
    public class WolfClient : IWolfClient, IWolfClientCacheAccessor, IDisposable
    {
        public string Url { get; }
        public string Token { get; }
        public string Device { get; }
        public bool IsConnected => this._client?.IsConnected == true;

        public bool UsersCachingEnabled
        {
            get => this.Caches?.UsersCachingEnabled == true;
            set => this.Caches.UsersCachingEnabled = value;
        }
        public bool GroupsCachingEnabled
        {
            get => this.Caches?.GroupsCachingEnabled == true;
            set => this.Caches.GroupsCachingEnabled = value;
        }
        public bool CharmsCachingEnabled
        {
            get => this.Caches?.CharmsCachingEnabled == true;
            set => this.Caches.CharmsCachingEnabled = value;
        }

        public event EventHandler Connected;
        public event EventHandler Disconnected;
        public event EventHandler<WolfMessageEventArgs> MessageReceived;
        public event EventHandler<WolfMessageSentEventArgs> MessageSent;
        public event EventHandler<UnhandledExceptionEventArgs> ErrorRaised;

        private readonly ISocketClient _client;
        private readonly MessageCallbackDispatcher _callbackDispatcher;
        protected ISerializerMap<string, IMessageSerializer> MessageSerializers { get; }
        protected ISerializerMap<Type, IResponseSerializer> ResponseSerializers { get; }
        protected IResponseTypeResolver ResponseTypeResolver { get; }
        protected ILogger Log { get; }
        protected WolfEntityCacheContainer Caches { get; set; }

        private CancellationTokenSource _cts;
        public uint? CurrentUserID { get; protected set; }

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
            this.Log = logger;
            this.ResponseTypeResolver = responseTypeResolver ?? new DefaultResponseTypeResolver();
            this.MessageSerializers = messageSerializers ?? new DefaultMessageSerializerMap();
            this.ResponseSerializers = responseSerializers ?? new DefaultResponseSerializerMap();

            // init dispatcher
            this._callbackDispatcher = new MessageCallbackDispatcher();

            // init caches
            this.Caches = new WolfEntityCacheContainer();

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
            if (this.IsConnected)
                throw new InvalidOperationException("Already connected");

            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            return _client.ConnectAsync(
                new Uri(new Uri(this.Url), $"/socket.io/?token={this.Token}&device={this.Device.ToLower()}&EIO=3&transport=websocket"),
                _cts.Token);
        }

        public Task DisconnectAsync(CancellationToken cancellationToken = default)
            => _client.DisconnectAsync(cancellationToken);

        public virtual void Dispose()
            => (_client as IDisposable)?.Dispose();

        protected virtual void Clear()
        {
            this._cts?.Cancel();
            this._cts?.Dispose();
            this.CurrentUserID = default;
            this.Caches?.ClearAll();
        }
        #endregion

        #region Message Sending
        public async Task<TResponse> SendAsync<TResponse>(IWolfMessage message, CancellationToken cancellationToken = default) where TResponse : IWolfResponse
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (string.IsNullOrWhiteSpace(message.Command))
                throw new ArgumentException("Message command cannot be null, empty or whitespace", nameof(message));
            if (!this.IsConnected)
                throw new InvalidOperationException("Not connected");

            if (!MessageSerializers.TryFindMappedSerializer(message.Command, out IMessageSerializer serializer))
            {
                // try fallback simple serialization
                Log?.LogWarning("Serializer for command {Command} not found, using fallback one", message.Command);
                serializer = MessageSerializers.FallbackSerializer;
            }
            SerializedMessageData data = serializer.Serialize(message);

            uint msgId = await _client.SendAsync(message.Command, data.Payload, data.BinaryMessages, cancellationToken).ConfigureAwait(false);
            IWolfResponse response = await AwaitResponseAsync<TResponse>(msgId, message, cancellationToken).ConfigureAwait(false);
            if (response.IsError())
                throw new MessageSendingException(message, response);
            this.MessageSent?.Invoke(this, new WolfMessageSentEventArgs(message, response));
            return (TResponse)response;
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
                    Type responseType = ResponseTypeResolver?.GetMessageResponseType<TResponse>(sentMessage) ?? typeof(TResponse);
                    if (!ResponseSerializers.TryFindMappedSerializer(responseType, out IResponseSerializer serializer))
                    {
                        Log?.LogWarning("Serializer for response type {Type} not found, using fallback one", responseType.FullName);
                        serializer = ResponseSerializers.FallbackSerializer;
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
                    Log?.LogError(ex, "Exception has occured when handling socket response");
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
        protected virtual Task OnMessageSentInternalAsync(IWolfMessage message, IWolfResponse response, SerializedMessageData rawResponse, CancellationToken cancellationToken = default)
        {
            // if it's a login message, we can extract current user ID
            if (response is LoginResponse loginResponse)
                this.CurrentUserID = loginResponse.UserID;

            // if it's chat message, populate with response info to get timestamp
            else if (message is ChatMessage chatMsg && response is ChatResponse)
                rawResponse?.Payload?.First?.PopulateObject(chatMsg, "body");

            if (this.UsersCachingEnabled)
            {
                // update users cache if it's user profile message
                if (response is UserProfileResponse userProfileResponse && userProfileResponse.UserProfiles?.Any() == true)
                {
                    foreach (WolfUser user in userProfileResponse.UserProfiles)
                        this.Caches?.UsersCache?.AddOrReplaceIfChanged(user);
                }
            }

            if (this.GroupsCachingEnabled)
            {
                // update groups cache if it's group profile message
                if (response is GroupProfileResponse groupProfileResponse && groupProfileResponse.GroupProfiles?.Any() == true)
                {
                    foreach (WolfGroup group in groupProfileResponse.GroupProfiles)
                        this.Caches?.GroupsCache?.AddOrReplaceIfChanged(group);
                }

                // update group member list if one was requested
                else if (response is ListGroupMembersResponse groupMembersResponse && message is ListGroupMembersMessage groupMembersMessage && groupMembersResponse.GroupMembers?.Any() == true)
                {
                    WolfGroup cachedGroup = this.Caches?.GroupsCache?.Get(groupMembersMessage.GroupID);
                    try
                    {
                        if (cachedGroup != null)
                            EntityModificationHelper.ReplaceAllGroupMembers(cachedGroup, groupMembersResponse.GroupMembers);
                    }
                    catch (NotSupportedException)
                    {
                        Log?.LogWarning("Cannot update group members for group {GroupID} as the Members collection is read only", cachedGroup.ID);
                    }
                }
            }


            if (this.CharmsCachingEnabled)
            {
                // update charms cache if it's charms list
                if (response is ListCharmsResponse listCharmsResponse && listCharmsResponse.Charms?.Any() == true)
                {
                    foreach (WolfCharm charm in listCharmsResponse.Charms)
                        this.Caches?.CharmsCache?.AddOrReplace(charm);
                }
            }

            // TODO: handle other types
            return Task.CompletedTask;
        }
        #endregion

        #region Caching
        // doubling the methods as below allows for hiding interface members
        // while at once allowing overriding the implementations

        // interface proxies
        WolfUser IWolfClientCacheAccessor.GetCachedUser(uint id)
            => GetCachedUserInternal(id);
        WolfGroup IWolfClientCacheAccessor.GetCachedGroup(uint id)
            => GetCachedGroupInternal(id);
        WolfCharm IWolfClientCacheAccessor.GetCachedCharm(uint id)
            => GetCachedCharmInternal(id);

        // overridable methods
        protected virtual WolfUser GetCachedUserInternal(uint id)
        {
            if (!this.IsConnected)
                throw new InvalidOperationException("Not connected");
            if (!this.UsersCachingEnabled)
                return null;
            WolfUser result = this.Caches?.UsersCache?.Get(id);
            if (result != null)
                Log?.LogTrace("User {UserID} found in cache", id);
            return result;
        }
        protected virtual WolfGroup GetCachedGroupInternal(uint id)
        {
            if (!this.IsConnected)
                throw new InvalidOperationException("Not connected");
            if (!this.GroupsCachingEnabled)
                return null;
            WolfGroup result = this.Caches?.GroupsCache?.Get(id);
            if (result != null)
                Log?.LogTrace("Group {GroupID} found in cache", id);
            return result;
        }
        protected virtual WolfCharm GetCachedCharmInternal(uint id)
        {
            if (!this.IsConnected)
                throw new InvalidOperationException("Not connected");
            if (!this.CharmsCachingEnabled)
                return null;
            WolfCharm result = this.Caches?.CharmsCache?.Get(id);
            if (result != null)
                Log?.LogTrace("Charm {CharmID} found in cache", id);
            return result;
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
                    if (!MessageSerializers.TryFindMappedSerializer(command, out IMessageSerializer serializer))
                    {
                        // don't throw exception here, as doing so will kill the socket client loop
                        Log?.LogError("Serializer for command {Command} not found", command);
                        return;
                    }
                    // deserialize message
                    SerializedMessageData rawData = new SerializedMessageData(payload, e.BinaryMessages);
                    IWolfMessage msg = serializer.Deserialize(command, rawData);
                    if (msg == null)
                        return;

                    Log?.LogDebug("Message received: {Command}", command);
                    await OnMessageReceivedInternalAsync(msg, rawData, _cts.Token).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                // don't rethrow exception here, as doing so will kill the socket client loop
                Log?.LogError(ex, "Exception occured when handling received message");
                this.ErrorRaised?.Invoke(this, new UnhandledExceptionEventArgs(ex, false));
            }
        }

        protected virtual async Task OnMessageReceivedInternalAsync(IWolfMessage message, SerializedMessageData rawMessage, CancellationToken cancellationToken = default)
        {
            // update user presence
            if (this.UsersCachingEnabled)
            {
                if (message is PresenceUpdateEvent presenceUpdate)
                {
                    WolfUser cachedUser = this.Caches?.UsersCache?.Get(presenceUpdate.UserID);
                    if (cachedUser != null)
                    {
                        Log?.LogTrace("Updating cached user {UserID} presence", cachedUser.ID);
                        rawMessage.Payload.PopulateObject(cachedUser, "body");
                    }
                }
                else if (message is UserUpdateEvent userUpdatedEvent)
                {
                    WolfUser cachedUser = this.Caches?.UsersCache?.Get(userUpdatedEvent.UserID);
                    if (cachedUser == null || string.IsNullOrWhiteSpace(userUpdatedEvent.Hash) || cachedUser.Hash != userUpdatedEvent.Hash)
                    {
                        Log?.LogTrace("Updating user {UserID}", userUpdatedEvent.UserID);
                        await SendAsync<UserProfileResponse>(
                            new UserProfileMessage(new uint[] { userUpdatedEvent.UserID }, true, true),
                            cancellationToken).ConfigureAwait(false);
                    }
                }
            }

            if (this.GroupsCachingEnabled)
            {
                // update group audio count
                if (message is GroupAudioCountUpdateEvent groupAudioUpdate)
                {
                    WolfGroup cachedGroup = this.Caches?.GroupsCache?.Get(groupAudioUpdate.GroupID);
                    if (cachedGroup != null)
                    {
                        Log?.LogTrace("Updating cached group {GroupID} presence", cachedGroup.ID);
                        rawMessage.Payload.PopulateObject(cachedGroup.AudioCounts, "body");
                    }
                }

                // update group when change event by requesting group profile
                else if (message is GroupUpdateEvent groupUpdate)
                {
                    // trigger group download only if cached group has different hash
                    WolfGroup cachedGroup = this.Caches?.GroupsCache?.Get(groupUpdate.GroupID);
                    if (cachedGroup != null && cachedGroup.Hash != groupUpdate.Hash)
                    {
                        await SendAsync<GroupProfileResponse>(
                            new GroupProfileMessage(new uint[] { groupUpdate.GroupID }), cancellationToken).ConfigureAwait(false);
                    }
                }

                // update group member list when one joined
                else if (message is GroupJoinMessage groupMemberJoined)
                {
                    WolfGroup cachedGroup = this.Caches?.GroupsCache?.Get(groupMemberJoined.GroupID);
                    try
                    {
                        if (cachedGroup != null)
                            EntityModificationHelper.SetGroupMember(cachedGroup,
                                new WolfGroupMember(groupMemberJoined.UserID.Value, groupMemberJoined.Capabilities.Value));
                    }
                    catch (NotSupportedException)
                    {
                        Log?.LogWarning("Cannot update group members for group {GroupID} as the Members collection is read only", cachedGroup.ID);
                    }
                }

                // update group member list if one left
                else if (message is GroupLeaveMessage groupMemberLeft)
                {
                    WolfGroup cachedGroup = this.Caches?.GroupsCache?.Get(groupMemberLeft.GroupID);
                    try
                    {
                        if (cachedGroup != null)
                            EntityModificationHelper.RemoveGroupMember(cachedGroup, groupMemberLeft.UserID.Value);
                    }
                    catch (NotSupportedException)
                    {
                        Log?.LogWarning("Cannot update group members for group {GroupID} as the Members collection is read only", cachedGroup.ID);
                    }
                }

                // update group member capabilities if member was updated
                else if (message is GroupMemberUpdateEvent groupMemberUpdated)
                {
                    WolfGroup cachedGroup = this.Caches?.GroupsCache?.Get(groupMemberUpdated.GroupID);
                    try
                    {
                        if (cachedGroup != null)
                            EntityModificationHelper.SetGroupMember(cachedGroup,
                                new WolfGroupMember(groupMemberUpdated.UserID, groupMemberUpdated.Capabilities));
                    }
                    catch (NotSupportedException)
                    {
                        Log?.LogWarning("Cannot update group members for group {GroupID} as the Members collection is read only", cachedGroup.ID);
                    }
                }
            }

            // invoke events, unless this message is a self-sent chat message
            if (message is IChatMessage chatMessage && chatMessage.SenderID.Value == this.CurrentUserID)
                return;
            this.MessageReceived?.Invoke(this, new WolfMessageEventArgs(message));
            _callbackDispatcher.Invoke(message);
        }

        public void AddMessageListener(IMessageCallback listener)
            => _callbackDispatcher.Add(listener);
        public void RemoveMessageListener(IMessageCallback listener)
            => _callbackDispatcher.Remove(listener);

        private void OnClientMessageSent(object sender, SocketMessageEventArgs e)
        {
            TryLogMessageTrace(e, "Sent");
            if (TryParseCommandEvent(e.Message, out string command, out _))
                Log?.LogDebug("Message sent: {Command}", command);
        }

        private void OnClientConnected(object sender, EventArgs e)
        {
            Log?.LogInformation("Connected to {URL} as {Device}", this.Url, this.Device);
            this.Connected?.Invoke(this, EventArgs.Empty);
        }

        private void OnClientDisconnected(object sender, SocketClosedEventArgs e)
        {
            if (e.CloseStatus == System.Net.WebSockets.WebSocketCloseStatus.NormalClosure)
                Log?.LogInformation("Disconnected ({Description})", e.CloseStatusDescription);
            else
                Log?.LogWarning("Disconnected ungracefully ({Status}, {Description})", e.CloseStatus.ToString(), e.CloseStatusDescription);
            this.Clear();
            this.Disconnected?.Invoke(this, EventArgs.Empty);
        }

        private void OnClientError(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
                Log?.LogError(ex, "Socket client error: {Error}", ex.Message);
            else Log?.LogError("Socket client error: {Error}", e.ExceptionObject?.ToString());
            this.ErrorRaised?.Invoke(this, e);
        }
        #endregion

        #region Internal helpers
        protected static bool TryParseCommandEvent(SocketMessage message, out string command, out JToken payload)
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

        protected void TryLogMessageTrace(SocketMessageEventArgs e, string keyword)
        {
            if (Log?.IsEnabled(LogLevel.Trace) == true)
            {
                if (e.BinaryMessages?.Any() == true)
                {
                    string binaryMessages = string.Join("\n", e.BinaryMessages.Where(msg => msg?.Any() == true).Select(msg => Encoding.UTF8.GetString(msg)));
                    Log?.LogTrace($"{keyword} {{MessageType}}: {{Message}}\n{{BinaryMessages}}", e.Message.Type.ToString(), e.Message.ToString(), binaryMessages);
                }
                else
                    Log?.LogTrace($"{keyword} {{MessageType}}: {{Message}}", e.Message.Type.ToString(), e.Message.ToString());
            }
        }
        #endregion
    }
}
