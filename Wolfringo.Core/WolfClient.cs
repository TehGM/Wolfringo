using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TehGM.Wolfringo.Messages;
using TehGM.Wolfringo.Messages.Serialization;
using TehGM.Wolfringo.Messages.Serialization.Internal;
using TehGM.Wolfringo.Socket;
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
        public bool ThrowMissingSerializer { get; set; } = true;

        public event Action<IWolfMessage> MessageReceived;

        private readonly ISocketClient _client;
        private readonly IDictionary<string, IMessageSerializer> _serializers;

        public WolfClient(string url, string token, string device = DefaultDevice)
        {
            this.Url = url;
            this.Token = token;
            this.Device = device;

            this._serializers = GetDefaultMessageSerializers();
            this._client = new SocketClient();
        }

        public WolfClient(string token, string device = DefaultDevice)
            : this(DefaultUrl, token, device) { }

        public WolfClient(string device = DefaultDevice)
            : this(DefaultUrl, new DefaultWolfTokenProvider().GenerateToken(18), device) { }

        public Task ConnectAsync()
            => _client.ConnectAsync(new Uri(new Uri(this.Url), $"/socket.io/?token={this.Token}&device={this.Device}&EIO=3&transport=websocket"));

        public Task DisconnectAsync()
            => _client.DisconnectAsync();

        public Task SendAsync(IWolfMessage message)
        {
            JToken payload;
            if (_serializers.TryGetValue(message.Command, out IMessageSerializer serializer))
                payload = serializer.Serialize(message);
            else if (!ThrowMissingSerializer)
                throw new KeyNotFoundException($"Serializer for command {message.Command} not found");
            else
                // try fallback simple serialization
                payload = new JObject(new JProperty("body", JToken.FromObject(message, SerializationHelper.DefaultSerializer)));

            throw new NotImplementedException();
        }

        public void Dispose()
            => (_client as IDisposable).Dispose();

        private void _client_OnReceivedEvent(string command)
        {
            if (!_serializers.TryGetValue(command, out IMessageSerializer serializer))
            {
                if (ThrowMissingSerializer)
                    throw new KeyNotFoundException($"Serializer for command {command} not found");
                return;
            }

            IWolfMessage msg = serializer.Deserialize(command, null, null);
            this.MessageReceived?.Invoke(msg);
        }

        protected virtual IDictionary<string, IMessageSerializer> GetDefaultMessageSerializers()
        {
            return new Dictionary<string, IMessageSerializer>(StringComparer.OrdinalIgnoreCase)
            {
                { MessageCommands.Welcome, new JsonMessageSerializer<WelcomeMessage>() },
                { MessageCommands.Login, new JsonMessageSerializer<LoginMessage>() },
                { MessageCommands.SubscribeToPm, new JsonMessageSerializer<SubscribeToPmMessage>() }
            };
        }

        public void SetSerializer(string command, IMessageSerializer serializer)
            => this._serializers[command] = serializer;
    }
}
