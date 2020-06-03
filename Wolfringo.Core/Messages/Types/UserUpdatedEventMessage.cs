using Newtonsoft.Json;

namespace TehGM.Wolfringo.Messages
{
    public class UserUpdatedEventMessage : IWolfMessage
    {
        public string Command => MessageCommands.SubscriberUpdate;

        [JsonProperty("id")]
        public uint UserID { get; private set; }
        [JsonProperty("hash")]
        public string Hash { get; private set; }
    }
}
