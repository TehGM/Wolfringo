using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using TehGM.Wolfringo.Messages;
using TehGM.Wolfringo.Messages.Responses;
using TehGM.Wolfringo.Messages.Serialization;
using TehGM.Wolfringo.Utilities;
using TehGM.Wolfringo.Utilities.Internal;

namespace TehGM.Wolfringo.Hosting
{
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
        private readonly IOptionsMonitor<HostedWolfClientOptions> _options;
        private readonly ISerializerMap<string, IMessageSerializer> _messageSerializers;
        private readonly ISerializerMap<Type, IResponseSerializer> _responseSerializers;
        private readonly IResponseTypeResolver _responseTypeResolver;
        private readonly ITokenProvider _tokenProvider;

        // IWolfClient
        public event EventHandler Connected;
        public event EventHandler Disconnected;
        public event EventHandler<WolfMessageEventArgs> MessageReceived;
        public event EventHandler<WolfMessageSentEventArgs> MessageSent;
        public event EventHandler<UnhandledExceptionEventArgs> ErrorRaised;
        public uint? CurrentUserID => this._client?.CurrentUserID;
        public bool IsConnected => this._client != null && this._client.IsConnected;

        public HostedWolfClient(IOptionsMonitor<HostedWolfClientOptions> options, ILogger<HostedWolfClient> logger, ITokenProvider tokenProvider,
            ISerializerMap<string, IMessageSerializer> messageSerializers, ISerializerMap<Type, IResponseSerializer> responseSerializers,
            IResponseTypeResolver responseTypeResolver)
        {
            this._callbacks = new List<IMessageCallback>();

            this._log = logger;
            this._options = options;
            this._messageSerializers = messageSerializers;
            this._responseSerializers = responseSerializers;
            this._responseTypeResolver = responseTypeResolver;
            this._tokenProvider = tokenProvider;

            // when options change, we need to dispose existing client, and create new one with new options
            _options.OnChange(async (opts) =>
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

        /** CLIENT CREATING AND STRIPPING DOWN **/
        private void CreateClient()
        {
            _log?.LogTrace("Creating underlying client");
            HostedWolfClientOptions options = this._options.CurrentValue;
            string token = this.GetCurrentToken();

            // if token is null, use default client behaviour for token with token provider
            if (token == null)
                this._client = new WolfClient(options.ServerURL, options.Device, _log, _tokenProvider, _messageSerializers, _responseSerializers, _responseTypeResolver);
            // otherwise, reuse the token for new client
            else
                this._client = new WolfClient(options.ServerURL, options.Device, token, _log, _messageSerializers, _responseSerializers, _responseTypeResolver);

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

            // sub to events
            this._client.Disconnected += OnClientDisconnected;
            this._client.Connected += OnClientConntected;
            this._client.ErrorRaised += OnClientErrorRaised;
            this._client.MessageReceived += OnClientMessageReceived;
            this._client.MessageSent += OnClientMessageSent;
            this._client.AddMessageListener<WelcomeEvent>(OnWelcome);

            // store token for reconnections
            this._token = this._client.Token;
        }

        private string GetCurrentToken()
            => this._client?.Token ?? this._options.CurrentValue.Token ?? this._token;

        private async Task DisposeClientAsync()
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
                if (disposingClient.IsConnected)
                    await disposingClient.DisconnectAsync().ConfigureAwait(false);
            }
            finally
            {
                disposingClient.Dispose();
            }
        }

        /** AUTO-LOGIN **/
        private async void OnWelcome(WelcomeEvent welcome)
        {
            // check if auto login is enabled
            HostedWolfClientOptions options = this._options.CurrentValue;
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
            try
            {
                await this.SendAsync<LoginResponse>(
                    new LoginMessage(options.LoginEmail, options.LoginPassword, false), _connectionCancellationToken).ConfigureAwait(false);
                await this.SendAsync(new SubscribeToPmMessage(), _connectionCancellationToken).ConfigureAwait(false);
                await this.SendAsync(new SubscribeToGroupMessage(), _connectionCancellationToken).ConfigureAwait(false);
                _log?.LogInformation("Automatically logged in as {Login}", options.LoginEmail);
            }
            catch (Exception ex)
            {
                _log?.LogCritical(ex, "Exception occured when automatically logging in");
                this.ErrorRaised?.Invoke(this, new UnhandledExceptionEventArgs(ex, true));
            }
        }

        /** IHostedService **/
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
            }
            return Task.CompletedTask;
        }
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
                _log?.LogCritical(ex, "Exception occured when trying to disconnect as a hosted client");
                this.ErrorRaised?.Invoke(this, new UnhandledExceptionEventArgs(ex, true));
            }
            return Task.CompletedTask;
        }

        /** CONNECT AND DISCONNECT **/
        public async Task ConnectAsync(CancellationToken cancellationToken = default)
        {
            // public connection should always be done with locking to avoid race conditions
            await _clientLock.WaitAsync().ConfigureAwait(false);
            try
            {
                await this.ConnectInternalAsync(cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                _clientLock.Release();
            }
        }

        // connect without locking first
        // this method is required in case client is already locked as part of connection process
        // it should always be called by a method that already locked the client
        private Task ConnectInternalAsync(CancellationToken cancellationToken = default)
        {
            ThrowIfConnected();
            if (_client == null)
                CreateClient();

            this._connectionCancellationToken = cancellationToken;
            this._manuallyDisconnected = false;
            return _client.ConnectAsync(cancellationToken);
        }

        public async Task DisconnectAsync(CancellationToken cancellationToken = default)
        {
            // public disconnection should always be done with locking to avoid race conditions
            await _clientLock.WaitAsync().ConfigureAwait(false);
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

        private async void OnClientDisconnected(object sender, EventArgs e)
        {
            // lock first to avoid race conditions
            await _clientLock.WaitAsync().ConfigureAwait(false);
            try
            {
                // only reconnect if client exists, wasn't diconnected manually, and auto-reconnect is actually enabled
                if (this._client == null || this._manuallyDisconnected || !this._options.CurrentValue.AutoReconnect)
                    return;
                TimeSpan delay = this._options.CurrentValue.AutoReconnectDelay;
                if (delay > TimeSpan.Zero)
                    await Task.Delay(delay).ConfigureAwait(false);
                await this.ConnectInternalAsync(_connectionCancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // strip client and retry
                _log?.LogWarning(ex, "Failed to auto-reconnect, recreating underlying client");
                this.ErrorRaised?.Invoke(this, new UnhandledExceptionEventArgs(ex, false));
                try
                {
                    await DisposeClientAsync().ConfigureAwait(false);
                    await this.ConnectInternalAsync(_connectionCancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex2)
                {
                    _log?.LogCritical(ex2, "Exception occured when attempting to reconnect with recreated client");
                    this.ErrorRaised?.Invoke(this, new UnhandledExceptionEventArgs(ex2, true));
                }
            }
            finally
            {
                this.Disconnected?.Invoke(this, e);
                _clientLock.Release();
            }
        }

        /** EVENTS **/
        public void AddMessageListener(IMessageCallback listener)
        {
            lock (this._callbacks)
            {
                this._callbacks.Add(listener);
                _client?.AddMessageListener(listener);
            }
        }
        public void RemoveMessageListener(IMessageCallback listener)
        {
            lock (this._callbacks)
            {
                this._callbacks.Remove(listener);
                _client?.RemoveMessageListener(listener);
            }
        }

        private void OnClientMessageSent(object sender, WolfMessageSentEventArgs e)
            => this.MessageSent?.Invoke(this, e);
        private void OnClientMessageReceived(object sender, WolfMessageEventArgs e)
            => this.MessageReceived?.Invoke(this, e);
        private void OnClientErrorRaised(object sender, UnhandledExceptionEventArgs e)
            => this.ErrorRaised?.Invoke(this, e);
        private void OnClientConntected(object sender, EventArgs e)
            => this.Connected?.Invoke(this, e);

        /** SENDING **/
        public async Task<TResponse> SendAsync<TResponse>(IWolfMessage message, CancellationToken cancellationToken = default) where TResponse : IWolfResponse
        {
            await _clientLock.WaitAsync().ConfigureAwait(false);
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

        /** CACHES **/
        WolfUser IWolfClientCacheAccessor.GetCachedUser(uint id)
            => (_client as IWolfClientCacheAccessor).GetCachedUser(id);
        WolfGroup IWolfClientCacheAccessor.GetCachedGroup(uint id)
            => (_client as IWolfClientCacheAccessor).GetCachedGroup(id);
        WolfCharm IWolfClientCacheAccessor.GetCachedCharm(uint id)
            => (_client as IWolfClientCacheAccessor).GetCachedCharm(id);
        WolfAchievement IWolfClientCacheAccessor.GetCachedAchievement(WolfLanguage language, uint id)
            => (_client as IWolfClientCacheAccessor).GetCachedAchievement(language, id);

        /** UTILS **/
        private void ThrowIfNotConnected()
        {
            if (!this.IsConnected)
                throw new InvalidOperationException("Not connected");
        }

        private void ThrowIfConnected()
        {
            if (this.IsConnected)
                throw new InvalidOperationException("Already connected");
        }

        public void Dispose()
        {
            _manuallyDisconnected = true;
            _client?.Dispose();
            _client = null;
            _callbacks?.Clear();
            _clientLock?.Dispose();
        }
    }
}
