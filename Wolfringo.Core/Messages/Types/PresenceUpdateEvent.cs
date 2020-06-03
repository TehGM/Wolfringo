using Newtonsoft.Json;

namespace TehGM.Wolfringo.Messages
{
    public class PresenceUpdateEvent : IWolfMessage
    {
        public string Command => MessageCommands.PresenceUpdate;

        [JsonProperty("id")]
        public uint UserID { get; private set; }
        [JsonProperty("deviceType")]
        public WolfDevice Device { get; private set; }
        [JsonProperty("onlineState")]
        public WolfOnlineState OnlineState { get; private set; }

        [JsonConstructor]
        private PresenceUpdateEvent() { }
    }
}
