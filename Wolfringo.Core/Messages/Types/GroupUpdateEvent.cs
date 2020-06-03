using Newtonsoft.Json;

namespace TehGM.Wolfringo.Messages
{
    public class GroupUpdateEvent : IWolfMessage
    {
        [JsonIgnore]
        public string Command => MessageCommands.GroupUpdate;

        [JsonProperty("id")]
        public uint GroupID { get; private set; }
        [JsonProperty("hash")]
        public string Hash { get; private set; }

        [JsonConstructor]
        private GroupUpdateEvent() { }
    }
}
