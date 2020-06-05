using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
        private readonly List<IMessageCallback> _callbacks;
        private bool _autoReconnect = false;

        // services
        private readonly ILogger _log;
        private readonly IOptionsMonitor<WolfClientOptions> _options;
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

        public HostedWolfClient(IOptionsMonitor<WolfClientOptions> options, ILogger<WolfClient> logger, ITokenProvider tokenProvider,
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

            _options.OnChange(async (opts) =>
            {
                if (_client == null)
                    return;
                await StripDownClientAsync().ConfigureAwait(false);
                if (this._autoReconnect)
                    await this.ConnectAsync(_connectionCancellationToken).ConfigureAwait(false);
            });
        }

        private async Task CreateClientAsync()
        {
            await _clientLock.WaitAsync().ConfigureAwait(false);
            try
            {
                this._client = new WolfClient(_options.CurrentValue, _log, _tokenProvider,
                    _messageSerializers, _responseSerializers, _responseTypeResolver);
                for (int i = 0; i < _callbacks.Count; i++)
                    _callbacks.Add(_callbacks[i]);
                this._client.Disconnected += OnClientDisconnected;
                this._client.Connected += OnClientConntected;
                this._client.ErrorRaised += OnClientErrorRaised;
                this._client.MessageReceived += OnClientMessageReceived;
                this._client.MessageSent += OnClientMessageSent;
            }
            finally
            {
                _clientLock.Release();
            }
        }

        private async Task StripDownClientAsync()
        {
            await _clientLock.WaitAsync().ConfigureAwait(false);
            try
            {
                if (this._client == null)
                    return;
                // null it first so auto reconnection does not happen
                WolfClient disposingClient = this._client;
                this._client = null;
                // disconnect and dispose it
                if (disposingClient.IsConnected)
                    await disposingClient.DisconnectAsync().ConfigureAwait(false);
                disposingClient.Dispose();
            }
            finally
            {
                _clientLock.Release();
            }
        }

        Task IHostedService.StartAsync(CancellationToken cancellationToken)
            => this.ConnectAsync(cancellationToken);

        Task IHostedService.StopAsync(CancellationToken cancellationToken)
            => this.DisconnectAsync(cancellationToken);

        #region Proxy methods
        public async Task ConnectAsync(CancellationToken cancellationToken = default)
        {
            if (_client == null)
                await CreateClientAsync().ConfigureAwait(false);
            if (_client?.IsConnected == true)
                throw new InvalidOperationException("Already connected");

            this._connectionCancellationToken = cancellationToken;
            this._autoReconnect = true;
            await _client.ConnectAsync(cancellationToken).ConfigureAwait(false);
        }

        public Task DisconnectAsync(CancellationToken cancellationToken = default)
        {
            this._autoReconnect = false;
            return _client?.DisconnectAsync(cancellationToken);
        }

        public void AddMessageListener(IMessageCallback listener)
        {
            lock (this._callbacks)
            {
                this._callbacks.Add(listener);
                _client.AddMessageListener(listener);
            }
        }

        public void RemoveMessageListener(IMessageCallback listener)
        {
            lock (this._callbacks)
            {
                this._callbacks.Remove(listener);
                _client.RemoveMessageListener(listener);
            }
        }

        private async void OnClientDisconnected(object sender, EventArgs e)
        {
            try
            {
                if (this._client == null || !this._autoReconnect)
                    return;
                await this.ConnectAsync(_connectionCancellationToken).ConfigureAwait(false);
            }
            catch
            {
                // strip client and retry
                await StripDownClientAsync().ConfigureAwait(false);
                await this.ConnectAsync(_connectionCancellationToken).ConfigureAwait(false);
            }
            finally
            {
                this.Disconnected?.Invoke(this, e);
            }
        }
        #endregion

        #region Simple proxy methods
        public Task<TResponse> SendAsync<TResponse>(IWolfMessage message, CancellationToken cancellationToken = default) where TResponse : IWolfResponse
            => _client.SendAsync<TResponse>(message, cancellationToken);

        // cache
        WolfUser IWolfClientCacheAccessor.GetCachedUser(uint id)
            => (_client as IWolfClientCacheAccessor).GetCachedUser(id);

        WolfGroup IWolfClientCacheAccessor.GetCachedGroup(uint id)
            => (_client as IWolfClientCacheAccessor).GetCachedGroup(id);

        WolfCharm IWolfClientCacheAccessor.GetCachedCharm(uint id)
            => (_client as IWolfClientCacheAccessor).GetCachedCharm(id);

        WolfAchievement IWolfClientCacheAccessor.GetCachedAchievement(WolfLanguage language, uint id)
            => (_client as IWolfClientCacheAccessor).GetCachedAchievement(language, id);

        // events
        private void OnClientMessageSent(object sender, WolfMessageSentEventArgs e)
        {
            this.MessageSent?.Invoke(this, e);
        }

        private void OnClientMessageReceived(object sender, WolfMessageEventArgs e)
        {
            this.MessageReceived?.Invoke(this, e);
        }

        private void OnClientErrorRaised(object sender, UnhandledExceptionEventArgs e)
        {
            this.ErrorRaised?.Invoke(this, e);
        }

        private void OnClientConntected(object sender, EventArgs e)
        {
            this.Connected?.Invoke(this, e);
        }
        #endregion

        public void Dispose()
        {
            _autoReconnect = false;
            _client?.Dispose();
            _client = null;
            _callbacks?.Clear();
            _clientLock?.Dispose();
        }
    }
}
