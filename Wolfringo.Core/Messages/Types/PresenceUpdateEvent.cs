using Newtonsoft.Json;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>Event when a user changes presence state.</summary>
    public class PresenceUpdateEvent : IWolfMessage
    {
        /// <inheritdoc/>
        [JsonIgnore]
        public string Command => MessageCommands.PresenceUpdate;

        /// <summary>ID of the user.</summary>
        [JsonProperty("id")]
        public uint UserID { get; private set; }
        /// <summary>User's device.</summary>
        [JsonProperty("deviceType")]
        public WolfDevice Device { get; private set; }
        /// <summary>User's online state.</summary>
        [JsonProperty("onlineState")]
        public WolfOnlineState OnlineState { get; private set; }

        [JsonConstructor]
        private PresenceUpdateEvent() { }
    }
}
