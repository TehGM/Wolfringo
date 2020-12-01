using Newtonsoft.Json;

namespace TehGM.Wolfringo.Socket
{
    /// <summary>Represents Socket.IO session information.</summary>
    public class SocketSession
    {
        /// <summary>Session ID.</summary>
        [JsonProperty("sid")]
        public string ID { get; private set; }
        /// <summary>Interval at which the client should ping the server.</summary>
        [JsonProperty("pingInterval", Required = Required.Always)]
        public int PingInterval { get; private set; }
        /// <summary>Timeout for ping messages.</summary>
        [JsonProperty("pingTimeout")]
        public int PingTimeout { get; private set; }
    }
}
