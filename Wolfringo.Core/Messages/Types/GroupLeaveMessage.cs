using Newtonsoft.Json;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>Group leaving message.</summary>
    public class GroupLeaveMessage : IWolfMessage
    {
        /// <inheritdoc/>
        [JsonIgnore]
        public string EventName => MessageEventNames.GroupMemberDelete;

        /// <summary>ID of the group.</summary>
        [JsonProperty("groupId")]
        public uint GroupID { get; private set; }
        // receiving only
        /// <summary>User who left the group.</summary>
        [JsonProperty("subscriberId", NullValueHandling = NullValueHandling.Ignore)]
        public uint? UserID { get; private set; }

        [JsonConstructor]
        protected GroupLeaveMessage() { }

        /// <summary>Creates a message instance.</summary>
        /// <param name="groupID">ID of the group to leave.</param>
        public GroupLeaveMessage(uint groupID) : this()
        {
            this.GroupID = groupID;
        }
    }
}
