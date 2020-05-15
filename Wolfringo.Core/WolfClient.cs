using SocketIOClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TehGM.Wolfringo.Utilities;

namespace TehGM.Wolfringo
{
    public class WolfClient : IWolfClient, IDisposable
    {
        public const string DefaultUrl = "wss://v3-rc.palringo.com:3051";
        public const string DefaultDevice = "bot";

        public string Url { get; }
        public string Token { get; }
        public string Device { get; }

        public event Action Connected;
        public event Action Disconnected;
        public event Action PingSent;
        public event Action<TimeSpan> PongReceived;

        private readonly SocketIO _client;

        public WolfClient(string url, string token, string device = DefaultDevice)
        {
            this.Url = url;
            this.Token = token;
            this.Device = device;

            this._client = new SocketIO(this.Url);
            this._client.Parameters = new Dictionary<string, string>()
            {
                { "token", this.Token },
                { "device", this.Device }
            };

            this._client.OnReceivedEvent += _client_OnReceivedEvent;
            this._client.OnConnected += () => this.Connected?.Invoke();
            this._client.OnClosed += _ => this.Disconnected?.Invoke();
            this._client.OnPing += () => this.PingSent?.Invoke();
            this._client.OnPong += ts => this.PongReceived?.Invoke(ts);
        }

        public WolfClient(string token, string device = DefaultDevice)
            : this(DefaultUrl, token, device) { }

        public WolfClient(string device = DefaultDevice)
            : this(DefaultUrl, new DefaultWolfTokenProvider().GenerateToken(18), device) { }

        public Task ConnectAsync()
            => _client.ConnectAsync();

        public Task DisconnectAsync()
            => _client.CloseAsync();

        public void Dispose()
            => _client.Dispose();

        private void _client_OnReceivedEvent(string arg1, SocketIOClient.Arguments.ResponseArgs arg2)
        {
            throw new NotImplementedException();
        }
    }
}
