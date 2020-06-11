#if !NETCOREAPP3_0
using Microsoft.AspNetCore.Hosting;
#endif
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TehGM.Wolfringo.Messages;
using TehGM.Wolfringo.Messages.Responses;
using TehGM.Wolfringo.Messages.Serialization;
using TehGM.Wolfringo.Utilities;
using TehGM.Wolfringo.Utilities.Internal;

namespace TehGM.Wolfringo.Hosting
{
    /// <summary>A wrapper for <see cref="WolfClient"/> designed to use with .NET Core Host.</summary>
    /// <remarks><para>This wrapper uses <see cref="WolfClient"/> internally, and delegates all method calls and events.</para>
    /// <para>This class supports configuration changes by using <see cref="IOptionsMonitor{TOptions}"/>.
    /// Whenever settings are changed, existing client is disconnected and disposed, and a new one is constructed.<br/>
    /// All listeners and event registrations will be re-assigned to the new client. All caches will be empty when client is rebuilt.</para>
    /// <para>This class automatically handles reconnection, unless <see cref="HostedWolfClientOptions.AutoReconnect"/> is set to false.
    /// If the client is disconnected manually by calling <see cref="DisconnectAsync(CancellationToken)"/>, automatic reconnection will not happen.</para>
    /// <para>The connection will automatically start when Host starts the service. This means that in Hosted scenarios (ASP.NET Core/Generic Host),
    /// starting the connection manually is not necessary.</para>
    /// <para>The client will login automatically whenever a connection is established, provided 
    /// <see cref="HostedWolfClientOptions.LoginEmail"/> and <see cref="HostedWolfClientOptions.LoginPassword"/> are populated and 
    /// <see cref="HostedWolfClientOptions.AutoLogin"/> is set to true. Otherwise, manual login will be required.</para></remarks>
    /// <seealso cref="WolfClient"/>
    /// <seealso cref="IWolfClient"/>
    public class HostedWolfClient : IHostedWolfClient, IHostedService, IWolfClient, IWolfClientCacheAccessor, IDisposable
    {
        // internal working vars
        private WolfClient _client;
        private readonly SemaphoreSlim _clientLock = new SemaphoreSlim(1, 1);
        private CancellationToken _connectionCancellationToken;
        private readonly List<IMessageCallback> _callbacks;     // keep registered callbacks so they are reused when client recrestes
        private bool _manuallyDisconnected = false;             // set to false when reconnection was manual
        private string _token;                                  // keep token cached so it's reused when client is recreates

        // services
        private readonly ILogger _log;
        private readonly ILogger _underlyingClientLog;
        private readonly IOptionsMonitor<HostedWolfClientOptions> _options;
        private readonly ISerializerMap<string, IMessageSerializer> _messageSerializers;
        private readonly ISerializerMap<Type, IResponseSerializer> _responseSerializers;
        private readonly IResponseTypeResolver _responseTypeResolver;
        private readonly ITokenProvider _tokenProvider;
#if NETCOREAPP3_0
        private readonly IHostApplicationLifetime _hostLifetime;
#else
        private readonly IApplicationLifetime _hostLifetime;
#endif

        // event registrations
        private readonly IDisposable _exitingEventRegistration;
        private readonly IDisposable _optionsChangeEventRegistration;

        // IWolfClient
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
        /// <inheritdoc/>
        public uint? CurrentUserID => this._client?.CurrentUserID;
        /// <summary>Is underlying client created and connected?</summary>
        public bool IsConnected => this._client != null && this._client.IsConnected;

        /// <summary>Creates a new hosted client.</summary>
        /// <param name="options">Client configuration.</param>
        /// <param name="logger">Logger to log all log events.</param>
        /// <param name="tokenProvider">Wolf token generator.</param>
        /// <param name="messageSerializers">Map of message serializers.</param>
        /// <param name="responseSerializers">Map of response serializers.</param>
        /// <param name="responseTypeResolver">Resolver of message's response type.</param>
        public HostedWolfClient(IOptionsMonitor<HostedWolfClientOptions> options, ILogger<HostedWolfClient> logger, ILogger<WolfClient> underlyingClientLogger, ITokenProvider tokenProvider,
            ISerializerMap<string, IMessageSerializer> messageSerializers, ISerializerMap<Type, IResponseSerializer> responseSerializers,
            IResponseTypeResolver responseTypeResolver,
#if NETCOREAPP3_0
            IHostApplicationLifetime hostLifetime
#else
            IApplicationLifetime hostLifetime
#endif
            )
        {
            this._callbacks = new List<IMessageCallback>();

            this._log = logger;
            this._underlyingClientLog = underlyingClientLogger;
            this._options = options;
            this._messageSerializers = messageSerializers;
            this._responseSerializers = responseSerializers;
            this._responseTypeResolver = responseTypeResolver;
            this._tokenProvider = tokenProvider;
            this._hostLifetime = hostLifetime;

            // disconnect when closing
            this._exitingEventRegistration = this._hostLifetime.ApplicationStopping.Register(() =>
            {
                if (this.IsConnected)
                    _ = this.DisconnectAsync();
            });

            // when options change, we need to dispose existing client, and create new one with new options
            this._optionsChangeEventRegistration = _options.OnChange(async (opts) =>
            {
                // lock to avoid race conditions
                await _clientLock.WaitAsync().ConfigureAwait(false);
                try
                {
                    if (_client == null)
                        return;
                    _log?.LogDebug("Options changed, recreting client");
                    await DisposeClientAsync().ConfigureAwait(false);
                    // if it wasn't manually disconnected, reconnect it
                    if (!this._manuallyDisconnected)
                        await this.ConnectInternalAsync(_connectionCancellationToken).ConfigureAwait(false);
                }
                finally
                {
                    _clientLock.Release();
                }
            });
        }

        /* CLIENT CREATING AND STRIPPING DOWN */
        /// <summary>Creates a new underlying WolfClient.</summary>
        /// <remarks><para>This method will automatically re-add listener callbacks and listen to client's events.</para>
        /// <para>Underlying client config will be set as configured in <see cref="HostedWolfClientOptions"/>.</para>
        /// <para>The previously generated token will be reused.</para></remarks>
        private void CreateClient()
        {
            _log?.LogTrace("Creating underlying client");
            HostedWolfClientOptions options = this._options.CurrentValue;
            string token = this.GetCurrentToken();

            // if token is null, use default client behaviour for token with token provider
            if (token == null)
                this._client = new WolfClient(options.ServerURL, options.Device, _underlyingClientLog, _tokenProvider, _messageSerializers, _responseSerializers, _responseTypeResolver);
            // otherwise, reuse the token for new client
            else
                this._client = new WolfClient(options.ServerURL, options.Device, token, _underlyingClientLog, _messageSerializers, _responseSerializers, _responseTypeResolver);

            // sub to events
            this._client.AddMessageListener<WelcomeEvent>(OnWelcome);
            this._client.Disconnected += OnClientDisconnected;
            this._client.Connected += OnClientConntected;
            this._client.ErrorRaised += OnClientErrorRaised;
            this._client.MessageReceived += OnClientMessageReceived;
            this._client.MessageSent += OnClientMessageSent;

            // if there are any callbacks from previous client, reuse them as well
            lock (this._callbacks)
            {
                for (int i = 0; i < _callbacks.Count; i++)
                    this._client.AddMessageListener(_callbacks[i]);
            }

            // set caching options
            this._client.GroupsCachingEnabled = options.GroupsCachingEnabled;
            this._client.UsersCachingEnabled = options.UsersCachingEnabled;
            this._client.CharmsCachingEnabled = options.CharmsCachingEnabled;
            this._client.AchievementsCachingEnabled = options.AchievementsCachingEnabled;

            // store token for reconnections
            this._token = this._client.Token;
        }

        /// <summary>Gets memorized token.</summary>
        /// <remarks>If <see cref="HostedWolfClientOptions"/> has token set, it'll take priority.</remarks>
        /// <returns>Token to use when recreating underlying client.</returns>
        private string GetCurrentToken()
            => this._options.CurrentValue.Token ?? this._client?.Token ?? this._token;

        /// <summary>Disposes underlying client.</summary>
        /// <remarks>If underlying client is still connected, a disconnection will be attempted. Auto-reconnection won't be attempted.</remarks>
        private async Task DisposeClientAsync(CancellationToken cancellationToken = default)
        {
            if (this._client == null)
                return;
            _log?.LogTrace("Disposing underlying client");
            // null it first so auto reconnection does not happen
            WolfClient disposingClient = this._client;
            this._client = null;
            // disconnect and dispose it
            try
            {
                if (disposingClient != null)
                {
                    disposingClient.Disconnected -= OnClientDisconnected;
                    disposingClient.Connected -= OnClientConntected;
                    disposingClient.ErrorRaised -= OnClientErrorRaised;
                    disposingClient.MessageReceived -= OnClientMessageReceived;
                    disposingClient.MessageSent -= OnClientMessageSent;
                }
                lock (this._callbacks)
                {
                    for (int i = 0; i < _callbacks.Count; i++)
                        this._client.RemoveMessageListener(_callbacks[i]);
                }
                disposingClient?.RemoveMessageListener<WelcomeEvent>(OnWelcome);

                if (disposingClient?.IsConnected == true)
                    await disposingClient.DisconnectAsync(cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                disposingClient?.Dispose();
            }
        }

        /* AUTO-LOGIN */
        /// <summary>Automatically logs user in.</summary>
        /// <remarks>Login attempt will be aborted if <see cref="HostedWolfClientOptions.AutoLogin"/> is set to false.</remarks>
        /// <param name="welcome">The welcome event received from the client.</param>
        private async void OnWelcome(WelcomeEvent welcome)
        {
            try
            {
                HostedWolfClientOptions options = this._options.CurrentValue;
                string loggedInNickname;

                // if user is not logged in with this token, need to do the login!
                if (welcome.LoggedInUser == null)
                {
                    // check if auto login is enabled
                    if (!options.AutoLogin)
                        return;
                    // check all values are valid
                    if (string.IsNullOrWhiteSpace(options.LoginEmail))
                    {
                        _log?.LogError("Cannot auto-login: {PropertyName} is empty", nameof(options.LoginEmail));
                        return;
                    }
                    if (string.IsNullOrWhiteSpace(options.LoginPassword))
                    {
                        _log?.LogError("Cannot auto-login: {PropertyName} is empty", nameof(options.LoginPassword));
                        return;
                    }

                    // send login
                    _log?.LogDebug("Auto-login: {Login}", options.LoginEmail);
                    LoginResponse response = await this.SendAsync<LoginResponse>(
                        new LoginMessage(options.LoginEmail, options.LoginPassword, false), _connectionCancellationToken).ConfigureAwait(false);
                    loggedInNickname = response.Nickname;
                }
                else loggedInNickname = welcome.LoggedInUser.Nickname;

                // subscribe to all the things
                await (this).SendAsync(new SubscribeToPmMessage(), _connectionCancellationToken).ConfigureAwait(false);
                await (this).SendAsync(new SubscribeToGroupMessage(), _connectionCancellationToken).ConfigureAwait(false);
                _log?.LogInformation("Automatically logged in as {Nickname}", loggedInNickname);
            }
            catch (Exception ex)
            {
                _log?.LogError(ex, "Exception occured when automatically logging in");
                this.ErrorRaised?.Invoke(this, new UnhandledExceptionEventArgs(ex, true));
            }
        }

        /* IHostedService */
        /// <inheritdoc/>
        Task IHostedService.StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                // only connect if not already connected
                if (!this.IsConnected)
                    return this.ConnectAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _log?.LogCritical(ex, "Exception occured when trying to connect as a hosted client");
                this.ErrorRaised?.Invoke(this, new UnhandledExceptionEventArgs(ex, true));
                if (_options.CurrentValue.CloseOnCriticalError)
                    _hostLifetime?.StopApplication();
            }
            return Task.CompletedTask;
        }
        /// <inheritdoc/>
        Task IHostedService.StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                // only disconnect if connected
                if (this.IsConnected)
                    return this.DisconnectAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _log?.LogError(ex, "Exception occured when trying to disconnect as a hosted client");
                this.ErrorRaised?.Invoke(this, new UnhandledExceptionEventArgs(ex, true));
            }
            return Task.CompletedTask;
        }

        /* CONNECT AND DISCONNECT */
        /// <inheritdoc/>
        public async Task ConnectAsync(CancellationToken cancellationToken = default)
        {
            // public connection should always be done with locking to avoid race conditions
            await _clientLock.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                await this.ConnectInternalAsync(cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                _clientLock.Release();
            }
        }

        /// <summary>Connect without using the client lock.</summary>
        /// <remarks><para>This method is called by internal processes that attempt to establish a connection. 
        /// These processes usually lock the client, and for that reason cannot use <see cref="ConnectAsync(CancellationToken)"/>.</para>
        /// <para>If calling this method, lock the client first. Otherwise use <see cref="ConnectAsync(CancellationToken)"/>.</para></remarks>
        private Task ConnectInternalAsync(CancellationToken cancellationToken = default)
        {
            ThrowIfConnected();
            if (_client == null)
                CreateClient();

            this._connectionCancellationToken = cancellationToken;
            this._manuallyDisconnected = false;
            return _client.ConnectAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public async Task DisconnectAsync(CancellationToken cancellationToken = default)
        {
            // public disconnection should always be done with locking to avoid race conditions
            await _clientLock.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                ThrowIfNotConnected();
                this._manuallyDisconnected = true;
                await _client.DisconnectAsync(cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                _clientLock.Release();
            }
        }

        /// <summary>Attempts to automatically reconnect the client.</summary>
        /// <remarks><para>This method is invoked whenever underlying client disconnects.</para>
        /// <para>If first attempt of reconnection fails, underlying client will be recreated and attempt will be made again.
        /// If this fails, error will be logged and <see cref="ErrorRaised"/> invoked.</para>
        /// <para>Reconnection attempt will be aborted if client is disposed, disconnection was manual or 
        /// <see cref="HostedWolfClientOptions.AutoReconnect"/> is set to false.</para></remarks>
        private async void OnClientDisconnected(object sender, EventArgs e)
        {
            TimeSpan delay = this._options.CurrentValue.AutoReconnectDelay;
            // lock first to avoid race conditions
            await _clientLock.WaitAsync(_connectionCancellationToken).ConfigureAwait(false);
            try
            {
                // only reconnect if client exists, wasn't diconnected manually, and auto-reconnect is actually enabled
                if (this._client == null || this._manuallyDisconnected || !this._options.CurrentValue.AutoReconnect)
                    return;
                if (delay > TimeSpan.Zero)
                    await Task.Delay(delay, _connectionCancellationToken).ConfigureAwait(false);
                await this.ConnectInternalAsync(_connectionCancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex.LogAsWarning(this._log, "Failed to auto - reconnect, recreating underlying client"))
            {
                // strip client and retry
                this.ErrorRaised?.Invoke(this, new UnhandledExceptionEventArgs(ex, false));

                int reconnectAttempts = 1;
                while (reconnectAttempts < _options.CurrentValue.AutoReconnectAttempts)
                {
                    reconnectAttempts++;
                    bool lastAttempt = reconnectAttempts == _options.CurrentValue.AutoReconnectAttempts;
                    await Task.Delay(delay, _connectionCancellationToken).ConfigureAwait(false);
                    try
                    {
                        await DisposeClientAsync(_connectionCancellationToken).ConfigureAwait(false);
                        await this.ConnectInternalAsync(_connectionCancellationToken).ConfigureAwait(false);
                        break;
                    }
                    catch (Exception ex2) when (
                        (lastAttempt && ex.LogAsCritical(this._log, "Exception occured when attempting to reconnect with recreated client")) || 
                        ex.LogAsWarning(this._log, "Exception occured when attempting to reconnect with recreated client, this was the last attempt"))
                    {
                        if (lastAttempt)
                        {
                            this.ErrorRaised?.Invoke(this, new UnhandledExceptionEventArgs(ex2, true));
                            if (_options.CurrentValue.CloseOnCriticalError)
                                _hostLifetime?.StopApplication();
                        }
                    }
                }
            }
            finally
            {
                this.Disconnected?.Invoke(this, e);
                _clientLock.Release();
            }
        }

        /* EVENTS */
        /// <inheritdoc/>
        public void AddMessageListener(IMessageCallback listener)
        {
            lock (this._callbacks)
            {
                this._callbacks.Add(listener);
                _client?.AddMessageListener(listener);
            }
        }
        /// <inheritdoc/>
        public void RemoveMessageListener(IMessageCallback listener)
        {
            lock (this._callbacks)
            {
                this._callbacks.Remove(listener);
                _client?.RemoveMessageListener(listener);
            }
        }

        /// <summary>Raised when underlying client sends a message. Invokes <see cref="MessageSent"/>.</summary>
        private void OnClientMessageSent(object sender, WolfMessageSentEventArgs e)
            => this.MessageSent?.Invoke(this, e);
        /// <summary>Raised when underlying client receives a message. Invokes <see cref="MessageReceived"/>.</summary>
        private void OnClientMessageReceived(object sender, WolfMessageEventArgs e)
            => this.MessageReceived?.Invoke(this, e);
        /// <summary>Raised when underlying client raises an error. Invokes <see cref="ErrorRaised"/>.</summary>
        private void OnClientErrorRaised(object sender, UnhandledExceptionEventArgs e)
            => this.ErrorRaised?.Invoke(this, e);
        /// <summary>Raised when underlying client connects. Invokes <see cref="Connected"/>.</summary>
        private void OnClientConntected(object sender, EventArgs e)
            => this.Connected?.Invoke(this, e);

        /* SENDING */
        /// <inheritdoc/>
        public async Task<TResponse> SendAsync<TResponse>(IWolfMessage message, CancellationToken cancellationToken = default) where TResponse : IWolfResponse
        {
            await _clientLock.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                ThrowIfNotConnected();
            }
            finally
            {
                _clientLock.Release();
            }
            return await _client.SendAsync<TResponse>(message, cancellationToken).ConfigureAwait(false);
        }

        /* CACHES */
        /// <inheritdoc/>
        WolfUser IWolfClientCacheAccessor.GetCachedUser(uint id)
            => (_client as IWolfClientCacheAccessor)?.GetCachedUser(id);
        /// <inheritdoc/>
        WolfGroup IWolfClientCacheAccessor.GetCachedGroup(uint id)
            => (_client as IWolfClientCacheAccessor)?.GetCachedGroup(id);
        /// <inheritdoc/>
        WolfCharm IWolfClientCacheAccessor.GetCachedCharm(uint id)
            => (_client as IWolfClientCacheAccessor)?.GetCachedCharm(id);
        /// <inheritdoc/>
        WolfAchievement IWolfClientCacheAccessor.GetCachedAchievement(WolfLanguage language, uint id)
            => (_client as IWolfClientCacheAccessor)?.GetCachedAchievement(language, id);
        WolfGroup IWolfClientCacheAccessor.GetCachedGroup(string name)
            => (_client as IWolfClientCacheAccessor)?.GetCachedGroup(name);

        /* UTILS */
        /// <summary>Throws exception if underlying client is not connected.</summary>
        /// <exception cref="InvalidOperationException">Underlying client is null or not connected.</exception>
        private void ThrowIfNotConnected()
        {
            if (!this.IsConnected)
                throw new InvalidOperationException("Not connected");
        }

        /// <summary>Throws exception if underlying client is connected.</summary>
        /// <exception cref="InvalidOperationException">Underlying client is connected.</exception>
        private void ThrowIfConnected()
        {
            if (this.IsConnected)
                throw new InvalidOperationException("Already connected");
        }

        /// <summary>Disposes this client, underlying client and all related resources.</summary>
        public void Dispose()
        {
            _manuallyDisconnected = true;
            _optionsChangeEventRegistration?.Dispose();
            _exitingEventRegistration?.Dispose();
            DisposeClientAsync().GetAwaiter().GetResult();
            _client = null;
            _callbacks?.Clear();
            _clientLock?.Dispose();
        }
    }
}
