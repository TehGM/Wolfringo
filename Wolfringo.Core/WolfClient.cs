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
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace TehGM.Wolfringo
{
    /// <summary>A default base wolf client implementation.</summary>
    /// <remarks><para>This implementation, together with IHostedWolfClient from Wolfringo.Hosting package, is the default intended way to 
    /// use Wolfringo library. It's based on message and response serializers, which combined with constructor Dependency Injection
    /// provide high degree of customizability.</para>
    /// <para>Custom SocketIO client <see cref="Socket.SocketClient"/> is used by this library. This client only provides functionality required for
    /// Wolf, and may not be suitable for other scenarios.</para>
    /// <para>This implementation automatically handles cache updates whenever a correct type of message or response is sent or received.
    /// For this reason, it's important to be careful when overriding 
    /// <see cref="OnMessageSentAsync(IWolfMessage, IWolfResponse, SerializedMessageData, CancellationToken)"/> and
    /// <see cref="OnMessageReceivedAsync(IWolfMessage, SerializedMessageData, CancellationToken)"/>, as not calling base
    /// implementation (or not implementing replacement behaviour) might cause functionality loss.</para></remarks>
    public class WolfClient : IWolfClient, IWolfClientCacheAccessor, IDisposable
    {
        /// <summary>URL of the server.</summary>
        public string Url { get; }
        /// <summary>Device to pass to the server when connecting.</summary>
        public WolfDevice Device { get; }
        /// <summary>Is this client currently connected?</summary>
        public bool IsConnected => this.SocketClient?.IsConnected == true;
        /// <inheritdoc/>
        public uint? CurrentUserID { get; protected set; }

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

        /// <summary>Whether the client should skip raising events for messages it sent.</summary>
        protected bool IgnoreOwnChatMessages { get; }
        /// <summary>Token used with the connection.</summary>
        protected string Token { get; }
        /// <summary>Socket client used by this WOLF client.</summary>
        protected ISocketClient SocketClient { get; }
        /// <summary>Callbacks dispatcher used by this WOLF client.</summary>
        protected MessageCallbackDispatcher CallbackDispatcher { get; }
        /// <summary>Message serializers mapping used when serializing and deserializing messages.</summary>
        protected ISerializerProvider<string, IMessageSerializer> MessageSerializers { get; }
        /// <summary>Response serializers mapping used when deserializing responses.</summary>
        protected ISerializerProvider<Type, IResponseSerializer> ResponseSerializers { get; }
        /// <summary>Response type resolver used when deserializing responses.</summary>
        protected IResponseTypeResolver ResponseTypeResolver { get; }
        /// <summary>Logger for all log messages.</summary>
        protected ILogger Log { get; }
        /// <summary>Caches container.</summary>
        protected IWolfClientCache Caches { get; set; }

        private CancellationTokenSource _connectionCts;

        #region Constructors
        /// <summary>Creates a new wolf client instance.</summary>
        /// <param name="services">Services provider to resolve dependencies from.</param>
        /// <param name="options">Options for this client.</param>
        public WolfClient(IServiceProvider services, WolfClientOptions options)
        {
            // verify input
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            if (string.IsNullOrWhiteSpace(options.ServerURL))
                throw new ArgumentNullException(nameof(options.ServerURL));

            // resolve services
            IWolfTokenProvider tokenProvider = services.GetRequiredService<IWolfTokenProvider>();
            this.ResponseTypeResolver = services.GetRequiredService<IResponseTypeResolver>();
            this.MessageSerializers = services.GetRequiredService<ISerializerProvider<string, IMessageSerializer>>();
            this.ResponseSerializers = services.GetRequiredService<ISerializerProvider<Type, IResponseSerializer>>();
            this.Caches = services.GetRequiredService<IWolfClientCache>();
            this.Log = services.GetService<ILogger<WolfClient>>()
                ?? services.GetService<ILogger>()
                ?? services.GetService<ILoggerFactory>()?.CreateLogger<WolfClient>();

            // set options
            this.IgnoreOwnChatMessages = true;
            this.Url = options.ServerURL;
            this.Device = options.Device;
            this.Token = tokenProvider.GetToken();

            // init dispatcher
            this.CallbackDispatcher = new MessageCallbackDispatcher();

            // init socket client
            this.SocketClient = services.GetRequiredService<ISocketClient>();
            this.SocketClient.MessageReceived += OnClientMessageReceived;
            this.SocketClient.MessageSent += OnClientMessageSent;
            this.SocketClient.Connected += OnClientConnected;
            this.SocketClient.Disconnected += OnClientDisconnected;
            this.SocketClient.ErrorRaised += OnClientError;
        }

        // backwards compatibility constructor overloads
        /// <summary>Creates a new wolf client instance.</summary>
        [Obsolete("Use WolfClientBuilder instead")]
        public WolfClient() : this(BuildDefaultServiceProvider(null), new WolfClientOptions()) { }

        /// <summary>Creates a new wolf client instance.</summary>
        /// <param name="log">Logger to use with this client. If null, no messages will be logged.</param>
        [Obsolete("Use WolfClientBuilder instead")]
        public WolfClient(ILogger log)
            : this(BuildDefaultServiceProvider(log), new WolfClientOptions()) { }

        /// <summary>Creates a new wolf client instance.</summary>
        /// <param name="logFactory">Logger factory to use with this client. If null, no messages will be logged.</param>
        [Obsolete("Use WolfClientBuilder instead")]
        public WolfClient(ILoggerFactory logFactory)
            : this(BuildDefaultServiceProvider(logFactory?.CreateLogger<WolfClient>()), new WolfClientOptions()) { }

        /// <summary>Builds default service provider. Used to temporarily support obsolete non-builder constructors.</summary>
        /// <param name="log">A logger to add to the services. If null, logging will be disabled.</param>
        /// <returns>A <see cref="SimpleServiceProvider"/> with default services added.</returns>
        protected static IServiceProvider BuildDefaultServiceProvider(ILogger log = null)
        {
            IDictionary<Type, object> services = new Dictionary<Type, object>()
            {
                { typeof(IWolfTokenProvider), new RandomizedWolfTokenProvider() },
                { typeof(IResponseTypeResolver), new ResponseTypeResolver() },
                { typeof(ISerializerProvider<string, IMessageSerializer>), new MessageSerializerProvider() },
                { typeof(ISerializerProvider<Type, IResponseSerializer>), new ResponseSerializerProvider() },
                { typeof(IWolfClientCache), new WolfEntityCacheContainer(new WolfCacheOptions(), log) },
                { typeof(ISocketClient), new SocketClient() }
            };
            if (log != null)
            {
                if (log is ILogger<WolfClient> typedLog)
                    services.Add(typeof(ILogger<WolfClient>), typedLog);
                services.Add(typeof(ILogger), log);
            }
            return new SimpleServiceProvider(services);
        }
        #endregion

        #region Connection management
        /// <inheritdoc/>
        /// <param name="device">Device to connect as.</param>
        /// <param name="cancellationToken">Cancellation token that can be used for Task cancellation.</param>
        public Task ConnectAsync(WolfDevice device, CancellationToken cancellationToken = default)
        {
            if (this.IsConnected)
                throw new InvalidOperationException("Already connected");

            Log?.LogDebug("Connecting");
            this.Clear();
            this._connectionCts = new CancellationTokenSource();
            return SocketClient.ConnectAsync(
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
            return SocketClient.DisconnectAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public virtual void Dispose()
        {
            this.Clear();
            (SocketClient as IDisposable)?.Dispose();
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
            if (string.IsNullOrWhiteSpace(message.EventName))
                throw new ArgumentException("Message command cannot be null, empty or whitespace", nameof(message));
            if (!this.IsConnected)
                throw new InvalidOperationException("Not connected");

            using (CancellationTokenSource sendingCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _connectionCts.Token))
            {
                Log?.LogTrace("Sending {Command}", message.EventName);
                // select serializer
                if (!MessageSerializers.TryFindSerializer(message.EventName, out IMessageSerializer serializer))
                {
                    // try fallback simple serialization
                    Log?.LogWarning("Serializer for command {Command} not found, using fallback one", message.EventName);
                    serializer = MessageSerializers.FallbackSerializer;
                }
                // serialize and send message
                SerializedMessageData data = serializer.Serialize(message);
                uint msgID = await SocketClient.SendAsync(message.EventName, data.Payload, data.BinaryMessages, sendingCts.Token).ConfigureAwait(false);
                IWolfResponse response = await AwaitResponseAsync<TResponse>(msgID, message, sendingCts.Token).ConfigureAwait(false);
                if (response.IsError())
                    throw new MessageSendingException(message, response);
                this.MessageSent?.Invoke(this, new WolfMessageSentEventArgs(message, response));
                return (TResponse)response;
            }
        }

        /// <summary>Waits for response for sent message.</summary>
        /// <remarks><para>If client uses <see cref="Messages.Responses.ResponseTypeResolver"/>, the type of response provided with 
        /// <see cref="ResponseTypeAttribute"/> on <paramref name="sentMessage"/> will be used for deserialization, 
        /// and <typeparamref name="TResponse"/> will be used only for casting. If <see cref="ResponseTypeAttribute"/> is not set on
        /// <paramref name="sentMessage"/>, <typeparamref name="TResponse"/> will be used for deserialization as normal.</para></remarks>
        /// <typeparam name="TResponse">Response type to cast response to.</typeparam>
        /// <param name="messageID">Sent message ID.</param>
        /// <param name="sentMessage">Sent message.</param>
        /// <param name="cancellationToken">Cancellation token that can be used for Task cancellation.</param>
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
                    if (!ResponseSerializers.TryFindSerializer(responseType, out IResponseSerializer serializer))
                    {
                        Log?.LogWarning("Serializer for response type {Type} not found, using fallback one", responseType.FullName);
                        serializer = ResponseSerializers.FallbackSerializer;
                    }
                    SerializedMessageData responseData = new SerializedMessageData(e.Message.Payload, e.BinaryMessages);
                    IWolfResponse response = serializer.Deserialize(responseType, responseData);

                    if (!response.IsError())
                    {
                        // if it's a login message, we can extract current user ID
                        if (response is LoginResponse loginResponse)
                            this.CurrentUserID = loginResponse.User.ID;

                        // when logging out, null the user ID.
                        else if (sentMessage is LogoutMessage)
                            this.CurrentUserID = null;

                        // if it's chat message, populate with response info to get timestamp
                        else if (sentMessage is ChatMessage chatMsg && response is ChatResponse)
                            responseData?.Payload?.First?.PopulateObject(chatMsg, "body");

                        // cache
                        await this.Caches.HandleMessageSentAsync(this, sentMessage, response, responseData, cancellationToken).ConfigureAwait(false);
                    }

                    // notify child classes
                    await OnMessageSentAsync(sentMessage, response, responseData, cancellationToken).ConfigureAwait(false);

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
            SocketClient.MessageReceived += callback;
            return tcs.Task;
        }

        /// <summary>Internal method for handling additional actions on sent message.</summary>
        /// <remarks>This method is invoked after client parses current user (for login/logout), populates chat message body data, and caches the entities.</remarks>
        /// <param name="message">Sent message.</param>
        /// <param name="response">Response received.</param>
        /// <param name="rawResponse">Raw response data.</param>
        /// <param name="cancellationToken">Cancellation token that can be used for Task cancellation.</param>
        protected virtual Task OnMessageSentAsync(IWolfMessage message, IWolfResponse response, SerializedMessageData rawResponse, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
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
            WolfCharm result = this.Caches?.CharmsCache?.Get(id);
            if (result != null)
                Log?.LogTrace("Charm {CharmID} found in cache", id);
            return result;
        }
        /// <summary>Get achievement from cache.</summary>
        /// <param name="language">Language of the achievement data.</param>
        /// <param name="id">ID of the achievement.</param>
        /// <returns>Cached achievement if found; otherwise null.</returns>
        /// <exception cref="InvalidOperationException">Not connected.</exception>
        protected virtual WolfAchievement GetCachedAchievementInternal(WolfLanguage language, uint id)
        {
            if (!this.IsConnected)
                throw new InvalidOperationException("Not connected");
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
                    if (!MessageSerializers.TryFindSerializer(command, out IMessageSerializer serializer))
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

                    // if welcome is already logged in, we can populate userID
                    if (msg is WelcomeEvent welcome && welcome.LoggedInUser != null)
                        this.CurrentUserID = welcome.LoggedInUser.ID;

                    // cache
                    await this.Caches.HandleMessageReceivedAsync(this, msg, rawData, _connectionCts.Token).ConfigureAwait(false);

                    // notify child classes
                    await OnMessageReceivedAsync(msg, rawData, _connectionCts.Token).ConfigureAwait(false);

                    // invoke events, unless this message is a self-sent chat message
                    if (msg is IChatMessage chatMessage && this.IgnoreOwnChatMessages && chatMessage.SenderID.Value == this.CurrentUserID)
                        return;
                    this.MessageReceived?.Invoke(this, new WolfMessageEventArgs(msg));
                    CallbackDispatcher.Invoke(msg);
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
        /// <remarks>This method is invoked after client parses current user (for welcome) and caches the entities.</remarks>
        /// <param name="message">Received message.</param>
        /// <param name="rawMessage">Raw received message.</param>
        /// <param name="cancellationToken">Cancellation token that can be used for Task cancellation.</param>
        protected virtual Task OnMessageReceivedAsync(IWolfMessage message, SerializedMessageData rawMessage, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        /// <inheritdoc/>
        public void AddMessageListener(IMessageCallback listener)
            => CallbackDispatcher.Add(listener);
        /// <inheritdoc/>
        public void RemoveMessageListener(IMessageCallback listener)
            => CallbackDispatcher.Remove(listener);

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
