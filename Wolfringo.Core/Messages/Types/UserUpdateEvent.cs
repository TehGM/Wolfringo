using Newtonsoft.Json;

namespace TehGM.Wolfringo.Messages
{
    public class UserUpdateEvent : IWolfMessage
    {
        public string Command => MessageCommands.SubscriberUpdate;

        [JsonProperty("id")]
        public uint UserID { get; private set; }
        [JsonProperty("hash")]
        public string Hash { get; private set; }

        [JsonConstructor]
        private UserUpdateEvent() { }
    }
}
