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
using TehGM.Wolfringo.Caching;
using TehGM.Wolfringo.Caching.Internal;
using System.Collections.Generic;
using Newtonsoft.Json;

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
        protected IWolfClientCache Cache { get; set; }
        /// <summary>Handler for tracking services that should be disposed when this <see cref="WolfClient"/> is being disposed.</summary>
        protected DisposableServicesHandler DisposablesHandler { get; }

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

            // resolve disposables handler
            this.DisposablesHandler = services.GetService<DisposableServicesHandler>() ?? new DisposableServicesHandler();

            // resolve services
            IWolfTokenProvider tokenProvider = this.DisposablesHandler.GetRequiredService<IWolfTokenProvider>(services);
            this.ResponseTypeResolver = this.DisposablesHandler.GetRequiredService<IResponseTypeResolver>(services);
            this.MessageSerializers = this.DisposablesHandler.GetRequiredService<ISerializerProvider<string, IMessageSerializer>>(services);
            this.ResponseSerializers = this.DisposablesHandler.GetRequiredService<ISerializerProvider<Type, IResponseSerializer>>(services);
            this.Cache = this.DisposablesHandler.GetRequiredService<IWolfClientCache>(services);
            this.Log = services.GetService<ILogger<WolfClient>>()
                ?? services.GetService<ILogger>()
                ?? services.GetService<ILoggerFactory>()?.CreateLogger<WolfClient>();

            // set options
            this.IgnoreOwnChatMessages = options.IgnoreOwnChatMessages;
            this.Url = options.ServerURL;
            this.Device = options.Device;
            this.Token = tokenProvider.GetToken();

            // init dispatcher
            this.CallbackDispatcher = new MessageCallbackDispatcher();

            // init socket client
            this.SocketClient = this.DisposablesHandler.GetRequiredService<ISocketClient>(services);
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
        /// <returns>A <see cref="IServiceProvider"/> with default services added.</returns>
        protected static IServiceProvider BuildDefaultServiceProvider(ILogger log = null)
        {
            // add all required services
            IServiceCollection services = new ServiceCollection();
            services.AddTransient<IWolfTokenProvider, RandomizedWolfTokenProvider>();
            services.AddSingleton<IResponseTypeResolver, ResponseTypeResolver>();
            services.AddSingleton<ISerializerProvider<string, IMessageSerializer>, MessageSerializerProvider>();
            services.AddSingleton<ISerializerProvider<Type, IResponseSerializer>, ResponseSerializerProvider>();
            services.AddSingleton<ISocketClient, SocketClient>();
            services.AddSingleton<IWolfClientCache>(provider
                => new WolfClientCache(new WolfCacheOptions(), provider.GetLoggerFor<WolfClientCache>()));

            if (log != null)
            {
                if (log is ILogger<WolfClient> typedLog)
                    services.AddSingleton<ILogger<WolfClient>>(typedLog);
                else if (log is ILogger<IWolfClient> interfaceTypedLog)
                    services.AddSingleton<ILogger<IWolfClient>>(interfaceTypedLog);
                services.AddSingleton<ILogger>(log);
            }

            // add tracker to know to dispose them
            DisposableServicesHandler handler = new DisposableServicesHandler();
            handler.MarkForDisposal<ISocketClient>();
            handler.MarkForDisposal<IWolfClientCache>();
            services.AddSingleton<DisposableServicesHandler>(handler);

            return services.BuildServiceProvider();
        }
        #endregion

        #region Connection management
        /// <inheritdoc/>
        /// <param name="device">Device to connect as.</param>
        /// <param name="cancellationToken">Cancellation token that can be used for Task cancellation.</param>
        public async Task ConnectAsync(WolfDevice device, CancellationToken cancellationToken = default)
        {
            if (this.IsConnected)
                throw new InvalidOperationException("Already connected");

            this.Log?.LogDebug("Connecting");
            this.Clear();
            await this.Cache.OnConnectingAsync(this, cancellationToken).ConfigureAwait(false);
            this._connectionCts = new CancellationTokenSource();
            await this.SocketClient.ConnectAsync(
                new Uri(new Uri(this.Url), $"/socket.io/?token={this.Token}&device={device.ToString().ToLowerInvariant()}&EIO=3&transport=websocket"),
                cancellationToken).ConfigureAwait(false);
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
            this.DisposablesHandler?.Dispose();
        }

        /// <summary>Clears all connection-bound variables.</summary>
        protected virtual void Clear()
        {
            try { this._connectionCts?.Cancel(); } catch { }
            try { this._connectionCts?.Dispose(); } catch { }
            this._connectionCts = null;
            this.CurrentUserID = null;
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

            using (CancellationTokenSource sendingCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, this._connectionCts?.Token ?? default))
            {
                this.Log?.LogTrace("Sending {Command}", message.EventName);
                // select serializer
                if (!this.MessageSerializers.TryFindSerializer(message.EventName, out IMessageSerializer serializer))
                {
                    // try fallback simple serialization
                    this.Log?.LogWarning("Serializer for command {Command} not found, using fallback one", message.EventName);
                    serializer =this.MessageSerializers.FallbackSerializer;
                }
                // serialize and send message
                SerializedMessageData data = serializer.Serialize(message);
                uint msgID = await this.SocketClient.SendAsync(message.EventName, data.Payload, data.BinaryMessages, sendingCts.Token).ConfigureAwait(false);
                IWolfResponse response = await this.AwaitResponseAsync<TResponse>(msgID, message, sendingCts.Token).ConfigureAwait(false);
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
                    Type responseType = this.ResponseTypeResolver?.GetMessageResponseType<TResponse>(sentMessage) ?? typeof(TResponse);
                    if (!this.ResponseSerializers.TryFindSerializer(responseType, out IResponseSerializer serializer))
                    {
                        this.Log?.LogWarning("Serializer for response type {Type} not found, using fallback one", responseType.FullName);
                        serializer = this.ResponseSerializers.FallbackSerializer;
                    }

                    SerializedMessageData responseData = new SerializedMessageData(e.Message.Payload, e.BinaryMessages);
                    if (responseData.IsError())
                    {
                        serializer = this.ResponseSerializers.FallbackSerializer;
                    }

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
                        await this.Cache.OnMessageSentAsync(this, sentMessage, response, responseData, cancellationToken).ConfigureAwait(false);
                    }

                    // notify child classes
                    await this.OnMessageSentAsync(sentMessage, response, responseData, cancellationToken).ConfigureAwait(false);

                    // set task result to finish it, and unhook the event to prevent memory leaks
                    tcs.TrySetResult(response);
                }
                catch (OperationCanceledException) when (this.LogWarning("Message sending aborted due to connection task being canceled")) { }
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
            this.SocketClient.MessageReceived += callback;
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
        /// <inheritdoc/>
        WolfUser IWolfClientCacheAccessor.GetCachedUser(uint id)
        {
            if (!this.IsConnected)
                throw new InvalidOperationException("Not connected");
            return this.Cache.GetCachedUser(id);
        }
        /// <inheritdoc/>
        WolfGroup IWolfClientCacheAccessor.GetCachedGroup(uint id)
        {
            if (!this.IsConnected)
                throw new InvalidOperationException("Not connected");
            return this.Cache.GetCachedGroup(id);
        }
        /// <inheritdoc/>
        WolfGroup IWolfClientCacheAccessor.GetCachedGroup(string name)
        {
            if (!this.IsConnected)
                throw new InvalidOperationException("Not connected");
            return this.Cache.GetCachedGroup(name);
        }
        /// <inheritdoc/>
        WolfCharm IWolfClientCacheAccessor.GetCachedCharm(uint id)
        {
            if (!this.IsConnected)
                throw new InvalidOperationException("Not connected");
            return this.Cache.GetCachedCharm(id);
        }
        /// <inheritdoc/>
        WolfAchievement IWolfClientCacheAccessor.GetCachedAchievement(WolfLanguage language, uint id)
        {
            if (!this.IsConnected)
                throw new InvalidOperationException("Not connected");
            return this.Cache.GetCachedAchievement(language, id);
        }
        #endregion

        #region Event handlers
        /// <summary>Deserializes received message and raises events and callbacks.</summary>
        /// <remarks>This method is invoked when underlying socket client receives a socket message.</remarks>
        private async void OnClientMessageReceived(object sender, SocketMessageEventArgs e)
        {
            this.TryLogMessageTrace(e, "Received");
            CancellationToken cancellationToken = this._connectionCts?.Token ?? default;

            if (TryParseCommandEvent(e.Message, out string command, out JToken payload))
            {
                using (this.Log?.BeginScope(new Dictionary<string, object>(2)
                {
                    { "Command", command },
                    { "Payload", payload?.ToString(Formatting.None) }
                }))
                {
                    try
                    {
                        // find serializer for command
                        if (!this.MessageSerializers.TryFindSerializer(command, out IMessageSerializer serializer))
                        {
                            // don't throw exception here, as doing so will kill the socket client loop
                            this.Log?.LogError("Serializer for command {Command} not found", command);
                            return;
                        }
                        // deserialize message
                        SerializedMessageData rawData = new SerializedMessageData(payload, e.BinaryMessages);
                        IWolfMessage msg = serializer.Deserialize(command, rawData);
                        if (msg == null)
                            return;

                        this.Log?.LogDebug("Message received: {Command}", command);

                        // if welcome is already logged in, we can populate userID
                        if (msg is WelcomeEvent welcome && welcome.LoggedInUser != null)
                            this.CurrentUserID = welcome.LoggedInUser.ID;

                        // cache
                        if (this.Cache != null)
                            await this.Cache.OnMessageReceivedAsync(this, msg, rawData, cancellationToken).ConfigureAwait(false);

                        // notify child classes
                        await this.OnMessageReceivedAsync(msg, rawData, cancellationToken).ConfigureAwait(false);

                        // invoke events, unless this message is a self-sent chat message
                        if (msg is IChatMessage chatMessage && this.IgnoreOwnChatMessages
                            && this.CurrentUserID != null && chatMessage.SenderID == this.CurrentUserID)
                            return;
                        this.MessageReceived?.Invoke(this, new WolfMessageEventArgs(msg));
                        this.CallbackDispatcher.Invoke(msg);
                    }
                    catch (OperationCanceledException) when (this.LogWarning("Message receiving aborted due to connection task being canceled")) { }
                    catch (Exception ex) when (this.LogUnhandledException(ex, e, command))
                    {
                        // don't rethrow exception here, as doing so will kill the socket client loop
                        this.ErrorRaised?.Invoke(this, new UnhandledExceptionEventArgs(ex, false));
                    }
                }
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
            => this.CallbackDispatcher.Add(listener);
        /// <inheritdoc/>
        public void RemoveMessageListener(IMessageCallback listener)
            => this.CallbackDispatcher.Remove(listener);

        /// <summary>Logs sent message. Invoked when underlying client sends a message.</summary>
        private void OnClientMessageSent(object sender, SocketMessageEventArgs e)
        {
            this.TryLogMessageTrace(e, "Sent");
            if (TryParseCommandEvent(e.Message, out string command, out _))
                this.Log?.LogDebug("Message sent: {Command}", command);
        }

        /// <summary>Logs connected and raises event. Invoked when underlying client connects to the server.</summary>
        private void OnClientConnected(object sender, EventArgs e)
        {
            this.Log?.LogInformation("Connected to {URL} as {Device}", this.Url, this.Device);
            this.Connected?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>Logs disconnected and raises event. Invoked when underlying client disconnects from the server.</summary>
        private void OnClientDisconnected(object sender, SocketClosedEventArgs e)
        {
            if (e.CloseStatus == WebSocketCloseStatus.NormalClosure)
            {
                // don't log message for operation canceled exception
                if (string.IsNullOrEmpty(e.CloseMessage) || (e.Exception is OperationCanceledException && e.CloseMessage == e.Exception?.Message))
                    this.Log?.LogInformation("Disconnected");
                else 
                    this.Log?.LogInformation("Disconnected ({Description})", e.CloseMessage);
            }
            else
                this.Log?.LogWarning("Disconnected ungracefully ({Status}, {Description})", e.CloseStatus.ToString(), e.CloseMessage ?? "-");

            this.Clear();
            this.Cache?.OnDisconnected(this, e);
            this.Disconnected?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>Logs error and raises event. Invoked when underlying client raises error event.</summary>
        private void OnClientError(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
                this.Log?.LogError(ex, "Socket client error: {Error}", ex.Message);
            else
                this.Log?.LogError("Socket client error: {Error}", e.ExceptionObject?.ToString());

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
            if (this.Log?.IsEnabled(LogLevel.Trace) != true)
                return;

            string binaryMessages = GetBinaryMessagesContents(e);
            if (!string.IsNullOrEmpty(binaryMessages))
                this.Log?.LogTrace($"{keyword} {{MessageType}}: {{Message}}\n{{BinaryMessages}}", e.Message.Type.ToString(), e.Message.ToString(), binaryMessages);
            else
                this.Log?.LogTrace($"{keyword} {{MessageType}}: {{Message}}", e.Message.Type.ToString(), e.Message.ToString());
        }

        private bool LogWarning(string message, params object[] args)
        {
            this.Log?.LogWarning(message, args);
            return true;
        }

        private bool LogUnhandledException(Exception ex, SocketMessageEventArgs e, string command)
        {
            if (!this.Log?.IsEnabled(LogLevel.Error) != true)
                return true;

            string binaryMessages = GetBinaryMessagesContents(e);
            using (string.IsNullOrEmpty(binaryMessages) ? null : this.Log?.BeginScope(new Dictionary<string, object>(1) { ["BinaryMessages"] = binaryMessages }))
            {
                this.Log?.LogError(ex, "Exception occured when handling received message with command {Command}", command);
                return true;
            }
        }

        private static string GetBinaryMessagesContents(SocketMessageEventArgs e)
        {
            if (e.BinaryMessages?.Any() != true)
                return null;

            return string.Join("\n", e.BinaryMessages.Where(msg => msg?.Any() == true).Select(msg => Encoding.UTF8.GetString(msg)));
        }
        #endregion
    }
}
