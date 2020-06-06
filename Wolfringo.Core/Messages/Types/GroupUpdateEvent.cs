using Newtonsoft.Json;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>An event when group profile changed.</summary>
    public class GroupUpdateEvent : IWolfMessage
    {
        /// <inheritdoc/>
        [JsonIgnore]
        public string Command => MessageCommands.GroupUpdate;

        /// <summary>ID of the group.</summary>
        [JsonProperty("id")]
        public uint GroupID { get; private set; }
        /// <summary>Entity state hash.</summary>
        [JsonProperty("hash")]
        public string Hash { get; private set; }

        [JsonConstructor]
        protected GroupUpdateEvent() { }
    }
}
