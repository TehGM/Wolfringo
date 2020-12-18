using Newtonsoft.Json;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>An event when group profile changed.</summary>
    public class GroupUpdateEvent : IWolfMessage
    {
        /// <inheritdoc/>
        /// <remarks>Equals to <see cref="MessageEventNames.GroupUpdate"/>.</remarks>
        [JsonIgnore]
        public string EventName => MessageEventNames.GroupUpdate;

        /// <summary>ID of the group.</summary>
        [JsonProperty("id")]
        public uint GroupID { get; private set; }
        /// <summary>Entity state hash.</summary>
        [JsonProperty("hash")]
        public string Hash { get; private set; }

        /// <summary>Creates a message instance.</summary>
        [JsonConstructor]
        protected GroupUpdateEvent() { }
    }
}
