using Newtonsoft.Json;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>Event when a group member has been stripped off their privileges.</summary>
    public class GroupMemberPrivilegedDeleteEvent : IWolfMessage
    {
        /// <inheritdoc/>
        /// <remarks>Equals to <see cref="MessageEventNames.GroupMemberPrivilegedDelete"/>.</remarks>
        [JsonIgnore]
        public string EventName => MessageEventNames.GroupMemberPrivilegedDelete;

        /// <summary>ID of the group.</summary>
        [JsonProperty("groupId")]
        public uint GroupID { get; private set; }
        /// <summary>Updated member's ID.</summary>
        [JsonProperty("subscriberId", NullValueHandling = NullValueHandling.Ignore)]
        public uint UserID { get; private set; }

        /// <summary>Creates a message instance.</summary>
        [JsonConstructor]
        protected GroupMemberPrivilegedDeleteEvent() { }
    }
}
