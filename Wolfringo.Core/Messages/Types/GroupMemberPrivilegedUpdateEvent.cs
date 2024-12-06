using Newtonsoft.Json;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>Event when a group member has been updated.</summary>
    public class GroupMemberPrivilegedUpdateEvent : IWolfMessage
    {
        /// <inheritdoc/>
        /// <remarks>Equals to <see cref="MessageEventNames.GroupMemberPrivilegedUpdate"/>.</remarks>
        [JsonIgnore]
        public string EventName => MessageEventNames.GroupMemberPrivilegedUpdate;

        /// <summary>ID of the group.</summary>
        [JsonProperty("groupId")]
        public uint GroupID { get; private set; }
        /// <summary>Updated member's ID.</summary>
        [JsonProperty("subscriberId", NullValueHandling = NullValueHandling.Ignore)]
        public uint UserID { get; private set; }
        /// <summary>Updated member's permissions.</summary>
        [JsonProperty("capabilities", NullValueHandling = NullValueHandling.Ignore)]
        public WolfGroupCapabilities Capabilities { get; private set; }

        /// <summary>Creates a message instance.</summary>
        [JsonConstructor]
        protected GroupMemberPrivilegedUpdateEvent() { }
    }
}
