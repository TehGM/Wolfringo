using Newtonsoft.Json;

namespace TehGM.Wolfringo.Messages
{
    public class GroupAudioCountUpdateEvent : IWolfMessage
    {
        public string Command => MessageCommands.GroupAudioCountUpdate;

        [JsonProperty("id")]
        public uint GroupID { get; private set; }
        [JsonProperty("broadcasterCount")]
        public int BroadcastersCount { get; private set; }
        [JsonProperty("consumerCount")]
        public int ListenersCount { get; private set; }

        [JsonConstructor]
        private GroupAudioCountUpdateEvent() { }
    }
}
