using Newtonsoft.Json;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>Event when a user profile updates.</summary>
    public class UserUpdateEvent : IWolfMessage
    {
        /// <inheritdoc/>
        [JsonIgnore]
        public string Command => MessageCommands.SubscriberUpdate;

        /// <summary>ID of the user.</summary>
        [JsonProperty("id")]
        public uint UserID { get; private set; }
        /// <summary>Entity state hash.</summary>
        [JsonProperty("hash")]
        public string Hash { get; private set; }

        [JsonConstructor]
        private UserUpdateEvent() { }
    }
}
