using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TehGM.Wolfringo.Messages;
using TehGM.Wolfringo.Messages.Responses;
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
        public WolfUser CurrentUser { get; private set; }

        public event Action<IWolfMessage> MessageReceived;

        private readonly ISocketClient _client;
        private readonly IDictionary<string, IMessageSerializer> _serializers;
        private readonly IMessageSerializer _fallbackSerializer;

        public WolfClient(string url, string token, string device = DefaultDevice)
        {
            this.Url = url;
            this.Token = token;
            this.Device = device;

            this._serializers = GetDefaultMessageSerializers();
            this._fallbackSerializer = new JsonMessageSerializer<IWolfMessage>();
            this._client = new SocketClient();
            this._client.MessageReceived += OnClientMessageReceived;
            this._client.MessageSent += OnClientMessageSent;
        }

        public WolfClient(string token, string device = DefaultDevice)
            : this(DefaultUrl, token, device) { }

        public WolfClient(string device = DefaultDevice)
            : this(DefaultUrl, new DefaultWolfTokenProvider().GenerateToken(18), device) { }

        public Task ConnectAsync()
            => _client.ConnectAsync(new Uri(new Uri(this.Url), $"/socket.io/?token={this.Token}&device={this.Device}&EIO=3&transport=websocket"));

        public Task DisconnectAsync()
            => _client.DisconnectAsync();

        public async Task<TResponse> SendAsync<TResponse>(IWolfMessage message) where TResponse : WolfResponse
        {
            SerializedMessageData data;
            if (_serializers.TryGetValue(message.Command, out IMessageSerializer serializer))
                data = serializer.Serialize(message);
            else if (!ThrowMissingSerializer)
                throw new KeyNotFoundException($"Serializer for command {message.Command} not found");
            else
                // try fallback simple serialization
                data = _fallbackSerializer.Serialize(message);

            uint msgId = await _client.SendAsync(message.Command, data.Payload, data.BinaryMessages).ConfigureAwait(false);
            return await AwaitResponseAsync<TResponse>(msgId).ConfigureAwait(false);
        }

        private Task<TResponse> AwaitResponseAsync<TResponse>(uint messageId) where TResponse : WolfResponse
        {
            TaskCompletionSource<TResponse> tcs = new TaskCompletionSource<TResponse>();
            EventHandler<SocketMessageEventArgs> callback = null;
            callback = (sender, e) =>
            {
                try
                {
                    // only accept response with corresponding message ID
                    if (e.Message.ID == null)
                        return;
                    if (e.Message.ID.Value != messageId)
                        return;

                    // parse response
                    JToken responseObject = (e.Message.Payload is JArray) ? e.Message.Payload.First : e.Message.Payload;
                    TResponse response = ParseResponse<TResponse>(responseObject);

                    // if it's a login message, we can also extract current user
                    if (response is LoginResponse loginResponse && loginResponse.Username != default)
                        this.CurrentUser = ParseResponse<WolfUser>(responseObject);

                    // set task result to finish it, and unhook the event to prevent memory leaks
                    tcs.TrySetResult(response);
                    if (_client != null)
                        _client.MessageReceived -= callback;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            };
            _client.MessageReceived += callback;
            return tcs.Task;

            T ParseResponse<T>(JToken response)
            {
                T result = response.ToObject<T>(SerializationHelper.DefaultSerializer);
                // if response has body or headers, further use it to populate the response entity
                response.PopulateObject(ref result, "headers");
                response.PopulateObject(ref result, "body");
                return result;
            }
        }

        public void Dispose()
            => (_client as IDisposable)?.Dispose();

        private void OnClientMessageReceived(object sender, SocketMessageEventArgs e)
        {
            try
            {
                Console.WriteLine($"< {e.Message}");

                if (e.Message.Type == SocketMessageType.BinaryEvent || e.Message.Type == SocketMessageType.Event)
                {
                    if (e.Message.Payload is JArray array)
                    {
                        // find serializer for command
                        string command = array.First.ToObject<string>();
                        if (!_serializers.TryGetValue(command, out IMessageSerializer serializer))
                        {
                            if (ThrowMissingSerializer)
                                throw new KeyNotFoundException($"Serializer for command {command} not found");
                            return;
                        }
                        // deserialize message
                        IWolfMessage msg = serializer.Deserialize(command, new SerializedMessageData(array.First.Next, e.BinaryMessages));
                        if (msg == null)
                            return;
                        // ignore own messages
                        if (msg is ChatMessage chatMessage && this.CurrentUser != null && chatMessage.SenderID.Value == this.CurrentUser.ID)
                            return;

                        this.MessageReceived?.Invoke(msg);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void OnClientMessageSent(object sender, SocketMessageEventArgs e)
        {
            Console.WriteLine($"> {e.Message}");
        }

        protected virtual IDictionary<string, IMessageSerializer> GetDefaultMessageSerializers()
        {
            return new Dictionary<string, IMessageSerializer>(StringComparer.OrdinalIgnoreCase)
            {
                { MessageCommands.Welcome, new JsonMessageSerializer<WelcomeMessage>() },
                { MessageCommands.Login, new JsonMessageSerializer<LoginMessage>() },
                { MessageCommands.SubscribeToPm, new JsonMessageSerializer<SubscribeToPmMessage>() },
                { MessageCommands.Chat, new ChatMessageSerializer() }
            };
        }

        public void SetSerializer(string command, IMessageSerializer serializer)
            => this._serializers[command] = serializer;
    }
}
