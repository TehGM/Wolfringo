using Newtonsoft.Json;

namespace TehGM.Wolfringo.Socket
{
    public class SocketSession
    {
        [JsonProperty("sid")]
        public string ID { get; private set; }
        [JsonProperty("pingInterval", Required = Required.Always)]
        public int PingInterval { get; private set; }
        [JsonProperty("pingTimeout")]
        public int PingTimeout { get; private set; }
    }
}
