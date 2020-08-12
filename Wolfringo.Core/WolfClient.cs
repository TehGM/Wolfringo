using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net.WebSockets;
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
    /// <summary>A default base wolf client implementation.</summary>
    /// <remarks><para>This implementation, together with IHostedWolfClient from Wolfringo.Hosting package, is the default intended way to 
    /// use Wolfringo library. It's based on message and response serializers, which combined with constructor Dependency Injection
    /// provide high degree of customizability.</para>
    /// <para>Custom SocketIO client <see cref="SocketClient"/> is used by this library. This client only provides functionality required for
    /// Wolf, and may not be suitable for other scenarios.</para>
    /// <para>This implementation automatically handles cache updates whenever a correct type of message or response is sent or received.
    /// For this reason, it's important to be careful when overriding 
    /// <see cref="OnMessageSentInternalAsync(IWolfMessage, IWolfResponse, SerializedMessageData, CancellationToken)"/> and
    /// <see cref="OnMessageReceivedInternalAsync(IWolfMessage, SerializedMessageData, CancellationToken)"/>, as not calling base
    /// implementation (or not implementing replacement behaviour) might cause functionality loss.</para></remarks>
    public class WolfClient : IWolfClient, IWolfClientCacheAccessor, IDisposable
    {
        /// <summary>Default Wolf server URL.</summary>
        public const string DefaultServerURL = "wss://v3-rc.palringo.com:3051";
        /// <summary>Default device string to pass to the server when connecting.</summary>
        public const WolfDevice DefaultDevice = WolfDevice.Bot;

        /// <summary>URL of the server.</summary>
        public string Url { get; }
        /// <summary>Token used with the connection.</summary>
        public string Token { get; }
        /// <summary>Device string to pass to the server when connecting.</summary>
        public WolfDevice Device { get; }
        /// <summary>Is this client currently connected?</summary>
        public bool IsConnected => this._client?.IsConnected == true;
        /// <inheritdoc/>
        public uint? CurrentUserID { get; protected set; }

        /// <summary>Enable or disable users cache.</summary>
        public bool UsersCachingEnabled
        {
            get => this.Caches?.UsersCachingEnabled == true;
            set => this.Caches.UsersCachingEnabled = value;
        }
        /// <summary>Enable or disable groups cache.</summary>
        public bool GroupsCachingEnabled
        {
            get => this.Caches?.GroupsCachingEnabled == true;
            set => this.Caches.GroupsCachingEnabled = value;
        }
        /// <summary>Enable or disable charms cache.</summary>
        public bool CharmsCachingEnabled
        {
            get => this.Caches?.CharmsCachingEnabled == true;
            set => this.Caches.CharmsCachingEnabled = value;
        }
        /// <summary>Enable or disable achievements cache.</summary>
        public bool AchievementsCachingEnabled
        {
            get => this.Caches?.AchievementsCachingEnabled == true;
            set => this.Caches.AchievementsCachingEnabled = value;
        }

        /// <inheritdoc/>
        public event EventHandler Connected;
        /// <inheritdoc/>
        public event EventHandler Disconnected;
        /// <inheritdoc/>
        public event EventHandler<WolfMessageEventArgs> MessageReceived;
        /// <inheritdoc/>
        public event EventHandler<WolfMessageSentEventArgs> MessageSent;
        /// <inheritdoc/>
        public event EventHandler<UnhandledExceptionEventArgs> ErrorRaised;

        private readonly ISocketClient _client;
        private readonly MessageCallbackDispatcher _callbackDispatcher;
        /// <summary>Message serializers mapping used when serializing and deserializing messages.</summary>
        protected ISerializerMap<string, IMessageSerializer> MessageSerializers { get; }
        /// <summary>Response serializers mapping used when deserializing responses.</summary>
        protected ISerializerMap<Type, IResponseSerializer> ResponseSerializers { get; }
        /// <summary>Response type resolver used when deserializing responses.</summary>
        protected IResponseTypeResolver ResponseTypeResolver { get; }
        /// <summary>Logger for all log messages.</summary>
        protected ILogger Log { get; }
        /// <summary>Caches container.</summary>
        protected WolfEntityCacheContainer Caches { get; set; }

        private CancellationTokenSource _connectionCts;

        #region Constructors
        /// <summary>Creates a new wolf client instance.</summary>
        /// <remarks><para>If any of the optional arguments is skipped or null, the following will be used:<br/>
        /// <paramref name="logger"/> - null (logging will be disabled)<br/>
        /// <paramref name="messageSerializers"/> - <see cref="DefaultMessageSerializerMap"/><br/>
        /// <paramref name="responseSerializers"/> - <see cref="DefaultResponseSerializerMap"/><br/>
        /// <paramref name="responseTypeResolver"/> - <see cref="DefaultResponseTypeResolver"/></para>
        /// <para>Both message and response serializers have a default fallback - if serializer for given message command/response type
        /// is not mapped, a default will be used. These fallback will log a warning when used. Note that message serializer
        /// uses fallback only when sending - when receiving, it'll log an error.</para></remarks>
        /// <param name="url">Wolf server URL. Needs to be WSS protocol.</param>
        /// <param name="device">Device to use when connecting.</param>
        /// <param name="token">Token to use for connection.</param>
        /// <param name="logger">Logger for logging logs.</param>
        /// <param name="messageSerializers">Message serializers mapping used when serializing and deserializing messages.</param>
        /// <param name="responseSerializers">Response serializers mapping used when deserializing responses.</param>
        /// <param name="responseTypeResolver">Response type resolver used when deserializing responses.</param>
        public WolfClient(string url, WolfDevice device, string token, ILogger logger = null, 
            ISerializerMap<string, IMessageSerializer> messageSerializers = null, ISerializerMap<Type, IResponseSerializer> responseSerializers = null, IResponseTypeResolver responseTypeResolver = null)
        {
            // verify input
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException(nameof(url));
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

        /// <summary>Creates a new wolf client instance.</summary>
        /// <remarks><para>If any of the optional arguments is skipped or null, the following will be used:<br/>
        /// <paramref name="logger"/> - null (logging will be disabled)<br/>
        /// <paramref name="tokenProvider"/> - <see cref="DefaultWolfTokenProvider"/><br/>
        /// <paramref name="messageSerializers"/> - <see cref="DefaultMessageSerializerMap"/><br/>
        /// <paramref name="responseSerializers"/> - <see cref="DefaultResponseSerializerMap"/><br/>
        /// <paramref name="responseTypeResolver"/> - <see cref="DefaultResponseTypeResolver"/></para>
        /// <para>Both message and response serializers have a default fallback - if serializer for given message command/response type
        /// is not mapped, a default will be used. These fallback will log a warning when used. Note that message serializer
        /// uses fallback only when sending - when receiving, it'll log an error.</para></remarks>
        /// <param name="url">Wolf server URL. Needs to be WSS protocol.</param>
        /// <param name="device">Device to use when connecting.</param>
        /// <param name="logger">Logger for logging logs.</param>
        /// <param name="tokenProvider">Token provider used to generate the token.</param>
        /// <param name="messageSerializers">Message serializers mapping used when serializing and deserializing messages.</param>
        /// <param name="responseSerializers">Response serializers mapping used when deserializing responses.</param>
        /// <param name="responseTypeResolver">Response type resolver used when deserializing responses.</param>
        public WolfClient(string url, WolfDevice device, ILogger logger = null, 
            ITokenProvider tokenProvider = null, 
            ISerializerMap<string, IMessageSerializer> messageSerializers = null, ISerializerMap<Type, IResponseSerializer> responseSerializers = null, IResponseTypeResolver responseTypeResolver = null)
            : this(url, device, GetNewToken(tokenProvider), logger, messageSerializers, responseSerializers, responseTypeResolver) { }

        /// <summary>Creates a new wolf client instance.</summary>
        /// <remarks><para>If any of the optional arguments is skipped or null, the following will be used:<br/>
        /// <paramref name="logger"/> - null (logging will be disabled)<br/>
        /// <paramref name="tokenProvider"/> - <see cref="DefaultWolfTokenProvider"/><br/>
        /// <paramref name="messageSerializers"/> - <see cref="DefaultMessageSerializerMap"/><br/>
        /// <paramref name="responseSerializers"/> - <see cref="DefaultResponseSerializerMap"/><br/>
        /// <paramref name="responseTypeResolver"/> - <see cref="DefaultResponseTypeResolver"/></para>
        /// <para>Both message and response serializers have a default fallback - if serializer for given message command/response type
        /// is not mapped, a default will be used. These fallback will log a warning when used. Note that message serializer
        /// uses fallback only when sending - when receiving, it'll log an error.</para></remarks>
        /// <param name="logger">Logger for logging logs.</param>
        /// <param name="tokenProvider">Token provider used to generate the token.</param>
        /// <param name="messageSerializers">Message serializers mapping used when serializing and deserializing messages.</param>
        /// <param name="responseSerializers">Response serializers mapping used when deserializing responses.</param>
        /// <param name="responseTypeResolver">Response type resolver used when deserializing responses.</param>
        public WolfClient(ILogger logger = null, 
            ITokenProvider tokenProvider = null,
            ISerializerMap<string, IMessageSerializer> messageSerializers = null, ISerializerMap<Type, IResponseSerializer> responseSerializers = null, IResponseTypeResolver responseTypeResolver = null)
            : this(DefaultServerURL, DefaultDevice, logger, tokenProvider, messageSerializers, responseSerializers, responseTypeResolver) { }

        /// <summary>Generates a new token using token provider.</summary>
        /// <remarks>If token provider is null, <see cref="DefaultWolfTokenProvider"/> will be used.</remarks>
        /// <param name="tokenProvider">Token provider to use when generating token.</param>
        /// <returns>Generated connection token.</returns>
        private static string GetNewToken(ITokenProvider tokenProvider = null)
        {
            if (tokenProvider == null)
                tokenProvider = new DefaultWolfTokenProvider();
            return tokenProvider.GenerateToken(18);
        }
        #endregion

        #region Connection management
        /// <inheritdoc/>
        /// <param name="device">Device to connect as.</param>
        public Task ConnectAsync(WolfDevice device, CancellationToken cancellationToken = default)
        {
            if (this.IsConnected)
                throw new InvalidOperationException("Already connected");

            Log?.LogDebug("Connecting");
            this.Clear();
            this._connectionCts = new CancellationTokenSource();
            return _client.ConnectAsync(
                new Uri(new Uri(this.Url), $"/socket.io/?token={this.Token}&device={device.ToString().ToLowerInvariant()}&EIO=3&transport=websocket"),
                cancellationToken);
        }

        /// <inheritdoc/>
        public Task ConnectAsync(CancellationToken cancellationToken = default)
            => this.ConnectAsync(this.Device, cancellationToken);

        /// <inheritdoc/>
        public Task DisconnectAsync(CancellationToken cancellationToken = default)
        {
            Log?.LogDebug("Disconnecting");
            return _client.DisconnectAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public virtual void Dispose()
        {
            this.Clear();
            (_client as IDisposable)?.Dispose();
        }

        /// <summary>Clears all connection-bound variables.</summary>
        protected virtual void Clear()
        {
            try { this._connectionCts?.Cancel(); } catch { }
            try { this._connectionCts?.Dispose(); } catch { }
            this._connectionCts = null;
            this.CurrentUserID = null;
            this.Caches?.ClearAll();
        }
        #endregion

        #region Message Sending
        /// <inheritdoc/>
        public async Task<TResponse> SendAsync<TResponse>(IWolfMessage message, CancellationToken cancellationToken = default) where TResponse : IWolfResponse
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (string.IsNullOrWhiteSpace(message.Command))
                throw new ArgumentException("Message command cannot be null, empty or whitespace", nameof(message));
            if (!this.IsConnected)
                throw new InvalidOperationException("Not connected");

            using (CancellationTokenSource sendingCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _connectionCts.Token))
            {
                Log?.LogTrace("Sending {Command}", message.Command);
                // select serializer
                if (!MessageSerializers.TryFindMappedSerializer(message.Command, out IMessageSerializer serializer))
                {
                    // try fallback simple serialization
                    Log?.LogWarning("Serializer for command {Command} not found, using fallback one", message.Command);
                    serializer = MessageSerializers.FallbackSerializer;
                }
                // serialize and send message
                SerializedMessageData data = serializer.Serialize(message);
                uint msgID = await _client.SendAsync(message.Command, data.Payload, data.BinaryMessages, sendingCts.Token).ConfigureAwait(false);
                IWolfResponse response = await AwaitResponseAsync<TResponse>(msgID, message, sendingCts.Token).ConfigureAwait(false);
                if (response.IsError())
                    throw new MessageSendingException(message, response);
                this.MessageSent?.Invoke(this, new WolfMessageSentEventArgs(message, response));
                return (TResponse)response;
            }
        }

        /// <summary>Waits for response for sent message.</summary>
        /// <remarks><para>If client uses <see cref="DefaultResponseTypeResolver"/>, the type of response provided with 
        /// <see cref="ResponseTypeAttribute"/> on <paramref name="message"/> will be used for deserialization, 
        /// and <typeparamref name="TResponse"/> will be used only for casting. If <see cref="ResponseTypeAttribute"/> is not set on
        /// <paramref name="message"/>, <typeparamref name="TResponse"/> will be used for deserialization as normal.</para></remarks>
        /// <typeparam name="TResponse">Response type to cast response to.</typeparam>
        /// <param name="messageID">Sent message ID.</param>
        /// <param name="sentMessage">Sent message.</param>
        /// <returns>Server's response.</returns>
        private Task<IWolfResponse> AwaitResponseAsync<TResponse>(uint messageID, IWolfMessage sentMessage, 
            CancellationToken cancellationToken = default) where TResponse : IWolfResponse
        {
            TaskCompletionSource<IWolfResponse> tcs = new TaskCompletionSource<IWolfResponse>();
            CancellationTokenRegistration ctr = cancellationToken.Register(() => tcs.TrySetCanceled(cancellationToken));
            EventHandler<SocketMessageEventArgs> callback = null;
            callback = async (sender, e) =>
            {
                // ignore messages that are no acks
                if (e.Message.Type != SocketMessageType.EventAck && e.Message.Type != SocketMessageType.BinaryEventAck)
                    return;
                // only accept response with corresponding message ID
                if (e.Message.ID == null || e.Message.ID.Value != messageID)
                    return;

                try
                {
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
                }
                catch (OperationCanceledException) when (LogWarning("Message sending aborted due to connection task being canceled")) { }
                catch (Exception ex) when (ex.LogAsError(this.Log, "Exception has occured when handling socket response"))
                {
                    // don't rethrow exception here, as doing so will kill the socket client loop
                    tcs.TrySetException(ex);
                }
                finally
                {
                    ctr.Dispose();
                    (sender as SocketClient).MessageReceived -= callback;
                }
            };
            _client.MessageReceived += callback;
            return tcs.Task;
        }

        /// <summary>Internal method for handling additional actions on sent message.</summary>
        /// <remarks>This implementation automatically handles cache updates whenever a correct type of message or response is sent or received.
        /// For this reason, it's important to be careful when overriding this method, as not calling base
        /// implementation (or not implementing replacement behaviour) might cause functionality loss.</remarks>
        /// <param name="message">Sent message.</param>
        /// <param name="response">Response received.</param>
        /// <param name="rawResponse">Raw response data.</param>
        protected virtual Task OnMessageSentInternalAsync(IWolfMessage message, IWolfResponse response, SerializedMessageData rawResponse, CancellationToken cancellationToken = default)
        {
            // don't do anything if response is not successful
            if (response.IsError())
                return Task.CompletedTask;

            // if it's a login message, we can extract current user ID
            if (response is LoginResponse loginResponse)
            {
                this.CurrentUserID = loginResponse.UserID;
                // subscribe to messages
                return Task.WhenAll(
                    this.SendAsync(new SubscribeToPmMessage(), cancellationToken),
                    this.SendAsync(new SubscribeToGroupMessage(), cancellationToken));
            }

            // when logging out, null the user ID.
            else if (message is LogoutMessage)
                this.CurrentUserID = null;

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
                    {
                        // repopulate group members if new group profile came without them
                        WolfGroup existingGroup = this.Caches?.GroupsCache?.Get(group.ID);
                        if (existingGroup != null && existingGroup.Members?.Any() == true && group.Members?.Any() != true)
                            EntityModificationHelper.ReplaceAllGroupMembers(group, existingGroup.Members.Values);
                        // replace cached group itself
                        this.Caches?.GroupsCache?.AddOrReplaceIfChanged(group);
                    }
                }

                // update group member list if one was requested
                else if (response is GroupMembersListResponse groupMembersResponse && message is GroupMembersListMessage groupMembersMessage && groupMembersResponse.GroupMembers?.Any() == true)
                {
                    WolfGroup cachedGroup = this.Caches?.GroupsCache?.Get(groupMembersMessage.GroupID);
                    try
                    {
                        if (cachedGroup != null)
                            EntityModificationHelper.ReplaceAllGroupMembers(cachedGroup, groupMembersResponse.GroupMembers);
                    }
                    catch (NotSupportedException) when (LogWarning("Cannot update group members for group {GroupID} as the Members collection is read only", cachedGroup.ID)) { }
                }

                // add group if it was created
                else if (response is GroupEditResponse groupEditResponse && message is GroupCreateMessage)
                    this.Caches?.GroupsCache.AddOrReplaceIfChanged(groupEditResponse.GroupProfile);

                // update group audio config
                else if (message is GroupAudioUpdateResponse groupAudioUpdateResponse && groupAudioUpdateResponse.AudioConfig != null)
                {
                    WolfGroup cachedGroup = this.Caches?.GroupsCache?.Get(groupAudioUpdateResponse.AudioConfig.GroupID);
                    if (cachedGroup != null)
                    {
                        Log?.LogTrace("Updating cached group {GroupID} audio config", cachedGroup.ID);
                        rawResponse.Payload.PopulateObject(cachedGroup.AudioConfig, "body");
                    }
                }
            }


            if (this.CharmsCachingEnabled)
            {
                // update charms cache if it's charms list
                if (response is CharmListResponse listCharmsResponse && listCharmsResponse.Charms?.Any() == true)
                {
                    foreach (WolfCharm charm in listCharmsResponse.Charms)
                        this.Caches?.CharmsCache?.AddOrReplace(charm);
                }
            }

            if (this.AchievementsCachingEnabled)
            {
                if (response is AchievementListResponse achievementListResponse && achievementListResponse.Achievements?.Any() == true &&
                    message is AchievementListMessage achievementListMessage)
                {
                    foreach (WolfAchievement achiv in achievementListResponse.GetFlattenedAchievementList())
                        this.Caches?.AchievementsCache?.AddOrReplace(achievementListMessage.Language, achiv);
                }
            }

            return Task.CompletedTask;
        }
        #endregion

        #region Caching
        // doubling the methods as below allows for hiding interface members
        // while at once allowing overriding the implementations

        // interface proxies
        /// <inheritdoc/>
        WolfUser IWolfClientCacheAccessor.GetCachedUser(uint id)
            => GetCachedUserInternal(id);
        /// <inheritdoc/>
        WolfGroup IWolfClientCacheAccessor.GetCachedGroup(uint id)
            => GetCachedGroupInternal(id);
        /// <inheritdoc/>
        WolfGroup IWolfClientCacheAccessor.GetCachedGroup(string name)
            => GetCachedGroupInternal(name);
        /// <inheritdoc/>
        WolfCharm IWolfClientCacheAccessor.GetCachedCharm(uint id)
            => GetCachedCharmInternal(id);
        /// <inheritdoc/>
        WolfAchievement IWolfClientCacheAccessor.GetCachedAchievement(WolfLanguage language, uint id)
            => GetCachedAchievementInternal(language, id);

        // overridable methods
        /// <summary>Get user from cache.</summary>
        /// <param name="id">ID of the user.</param>
        /// <returns>Cached user if found; otherwise null.</returns>
        /// <exception cref="InvalidOperationException">Not connected.</exception>
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
        /// <summary>Get group from cache.</summary>
        /// <param name="id">ID of the group.</param>
        /// <returns>Cached group if found; otherwise null.</returns>
        /// <exception cref="InvalidOperationException">Not connected.</exception>
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
        /// <summary>Get group from cache.</summary>
        /// <param name="name">ID of the group.</param>
        /// <returns>Cached group if found; otherwise null.</returns>
        /// <exception cref="InvalidOperationException">Not connected.</exception>
        protected virtual WolfGroup GetCachedGroupInternal(string name)
        {
            if (!this.IsConnected)
                throw new InvalidOperationException("Not connected");
            if (!this.GroupsCachingEnabled)
                return null;
            WolfGroup result = this.Caches?.GroupsCache?.Find(group => group.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (result != null)
                Log?.LogTrace("Group {GroupName} found in cache", name);
            return result;
        }
        /// <summary>Get charm from cache.</summary>
        /// <param name="id">ID of the charm.</param>
        /// <returns>Cached charm if found; otherwise null.</returns>
        /// <exception cref="InvalidOperationException">Not connected.</exception>
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
        /// <summary>Get achievement from cache.</summary>
        /// <param name="id">ID of the achievement.</param>
        /// <returns>Cached achievement if found; otherwise null.</returns>
        /// <exception cref="InvalidOperationException">Not connected.</exception>
        protected virtual WolfAchievement GetCachedAchievementInternal(WolfLanguage language, uint id)
        {
            if (!this.IsConnected)
                throw new InvalidOperationException("Not connected");
            if (!this.AchievementsCachingEnabled)
                return null;
            WolfAchievement result = this.Caches?.AchievementsCache?.Get(language, id);
            if (result != null)
                Log?.LogTrace("Achievement {AchievementID} found in cache", id);
            return result;
        }
        #endregion

        #region Event handlers
        /// <summary>Deserializes received message and raises events and callbacks.</summary>
        /// <remarks>This method is invoked when underlying socket client receives a socket message.</remarks>
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
                    await OnMessageReceivedInternalAsync(msg, rawData, _connectionCts.Token).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException) when (LogWarning("Message receiving aborted due to connection task being canceled")) { }
            catch (Exception ex) when (ex.LogAsError(this.Log, "Exception occured when handling received message"))
            {
                // don't rethrow exception here, as doing so will kill the socket client loop
                this.ErrorRaised?.Invoke(this, new UnhandledExceptionEventArgs(ex, false));
            }
        }

        /// <summary>Internal method for handling additional actions on received message.</summary>
        /// <remarks>This implementation automatically handles cache updates whenever a correct type of message or response is sent or received.
        /// Additionally, this method also raises events and callbacks, unless received message is a chat message sent by <see cref="CurrentUserID"/>.
        /// For this reason, it's important to be careful when overriding this method, as not calling base
        /// implementation (or not implementing replacement behaviour) might cause functionality loss.</remarks>
        /// <param name="message">Received message.</param>
        /// <param name="rawMessage">Raw received message.</param>
        protected virtual async Task OnMessageReceivedInternalAsync(IWolfMessage message, SerializedMessageData rawMessage, CancellationToken cancellationToken = default)
        {
            // if welcome is already logged in, we can populate userID
            if (message is WelcomeEvent welcome && welcome.LoggedInUser != null)
            {
                this.CurrentUserID = welcome.LoggedInUser.ID;
                await Task.WhenAll(
                    this.SendAsync(new SubscribeToPmMessage(), cancellationToken), 
                    this.SendAsync(new SubscribeToGroupMessage(), cancellationToken))
                    .ConfigureAwait(false);
            }

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
                if (message is GroupAudioCountUpdateEvent groupAudioCountUpdate)
                {
                    WolfGroup cachedGroup = this.Caches?.GroupsCache?.Get(groupAudioCountUpdate.GroupID);
                    if (cachedGroup != null)
                    {
                        Log?.LogTrace("Updating cached group {GroupID} audio counts", cachedGroup.ID);
                        rawMessage.Payload.PopulateObject(cachedGroup.AudioCounts, "body");
                    }
                }

                // update group audio config
                if (message is GroupAudioUpdateMessage groupAudioUpdate)
                {
                    WolfGroup cachedGroup = this.Caches?.GroupsCache?.Get(groupAudioUpdate.GroupID);
                    if (cachedGroup != null)
                    {
                        Log?.LogTrace("Updating cached group {GroupID} audio config", cachedGroup.ID);
                        rawMessage.Payload.PopulateObject(cachedGroup.AudioConfig, "body");
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
                    WolfGroup cachedGroup = this.Caches?.GroupsCache?.Get(groupMemberJoined.GroupID.Value);
                    try
                    {
                        if (cachedGroup != null)
                            EntityModificationHelper.SetGroupMember(cachedGroup,
                                new WolfGroupMember(groupMemberJoined.UserID.Value, groupMemberJoined.Capabilities.Value));
                    }
                    catch (NotSupportedException) when (LogWarning("Cannot update group members for group {GroupID} as the Members collection is read only", cachedGroup.ID)) { }
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
                    catch (NotSupportedException) when (LogWarning("Cannot update group members for group {GroupID} as the Members collection is read only", cachedGroup.ID)) { }
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
                    catch (NotSupportedException) when (LogWarning("Cannot update group members for group {GroupID} as the Members collection is read only", cachedGroup.ID)) { }
                }
            }

            // invoke events, unless this message is a self-sent chat message
            if (message is IChatMessage chatMessage && chatMessage.SenderID.Value == this.CurrentUserID)
                return;
            this.MessageReceived?.Invoke(this, new WolfMessageEventArgs(message));
            _callbackDispatcher.Invoke(message);
        }

        /// <inheritdoc/>
        public void AddMessageListener(IMessageCallback listener)
            => _callbackDispatcher.Add(listener);
        /// <inheritdoc/>
        public void RemoveMessageListener(IMessageCallback listener)
            => _callbackDispatcher.Remove(listener);

        /// <summary>Logs sent message. Invoked when underlying client sends a message.</summary>
        private void OnClientMessageSent(object sender, SocketMessageEventArgs e)
        {
            TryLogMessageTrace(e, "Sent");
            if (TryParseCommandEvent(e.Message, out string command, out _))
                Log?.LogDebug("Message sent: {Command}", command);
        }

        /// <summary>Logs connected and raises event. Invoked when underlying client connects to the server.</summary>
        private void OnClientConnected(object sender, EventArgs e)
        {
            Log?.LogInformation("Connected to {URL} as {Device}", this.Url, this.Device);
            this.Connected?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>Logs disconnected and raises event. Invoked when underlying client disconnects from the server.</summary>
        private void OnClientDisconnected(object sender, SocketClosedEventArgs e)
        {
            if (e.CloseStatus == WebSocketCloseStatus.NormalClosure)
            {
                // don't log message for operation canceled exception
                if (string.IsNullOrEmpty(e.CloseMessage) || (e.Exception is OperationCanceledException && e.CloseMessage == e.Exception?.Message))
                    Log?.LogInformation("Disconnected");
                else 
                    Log?.LogInformation("Disconnected ({Description})", e.CloseMessage);
            }
            else
                Log?.LogWarning("Disconnected ungracefully ({Status}, {Description})", e.CloseStatus.ToString(), e.CloseMessage ?? "-");

            this.Clear();
            this.Disconnected?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>Logs error and raises event. Invoked when underlying client raises error event.</summary>
        private void OnClientError(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
                Log?.LogError(ex, "Socket client error: {Error}", ex.Message);
            else
                Log?.LogError("Socket client error: {Error}", e.ExceptionObject?.ToString());

            this.ErrorRaised?.Invoke(this, e);
        }
        #endregion

        #region Internal helpers
        /// <summary>Tries to parse command and payload object.</summary>
        /// <param name="message">Received socket message.</param>
        /// <param name="command">Parsed message command; null if not a command message.</param>
        /// <param name="payload">Extracted payload.</param>
        /// <returns>True if command parsed and is not null; otherwise false.</returns>
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

        /// <summary>Logs socket message event with trace log level.</summary>
        /// <param name="e">Received socket event.</param>
        /// <param name="keyword">Keyword to prepend in log message, for example "Sent" or "Received".</param>
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

        private bool LogWarning(string message, params object[] args)
        {
            Log?.LogWarning(message, args);
            return true;
        }
        #endregion
    }
}
