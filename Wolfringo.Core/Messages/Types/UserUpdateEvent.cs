using Newtonsoft.Json;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>Event when a user profile updates.</summary>
    public class UserUpdateEvent : IWolfMessage
    {
        /// <inheritdoc/>
        /// <remarks>Equals to <see cref="MessageEventNames.SubscriberUpdate"/>.</remarks>
        [JsonIgnore]
        public string EventName => MessageEventNames.SubscriberUpdate;

        /// <summary>ID of the user.</summary>
        [JsonProperty("id")]
        public uint UserID { get; private set; }
        /// <summary>Entity state hash.</summary>
        [JsonProperty("hash")]
        public string Hash { get; private set; }

        /// <summary>Creates a message instance.</summary>
        [JsonConstructor]
        protected UserUpdateEvent() { }
    }
}
