using Newtonsoft.Json;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>A message for invoking a group action.</summary>
    /// <seealso cref="GroupActionChatEvent"/>
    public class GroupAdminMessage : IWolfMessage
    {
        /// <inheritdoc/>
        [JsonIgnore]
        public string Command => MessageCommands.GroupAdmin;

        /// <summary>ID of the group.</summary>
        [JsonProperty("groupId")]
        public uint GroupID { get; private set; }
        /// <summary>ID of group member that is target of the action.</summary>
        [JsonProperty("subscriberId", NullValueHandling = NullValueHandling.Ignore)]
        public uint UserID { get; private set; }
        /// <summary>Group member's new capabilities.</summary>
        [JsonProperty("capabilities", NullValueHandling = NullValueHandling.Ignore)]
        public WolfGroupCapabilities Capabilities { get; private set; }

        [JsonConstructor]
        private GroupAdminMessage() { }

        /// <summary>Creates a message instance.</summary>
        /// <param name="userID">ID of group member to perform action on.</param>
        /// <param name="groupID">ID of group to perform action in.</param>
        /// <param name="newCapabilities">Group member's new capabilities.</param>
        public GroupAdminMessage(uint userID, uint groupID, WolfGroupCapabilities newCapabilities)
        {
            this.GroupID = groupID;
            this.UserID = userID;
            this.Capabilities = newCapabilities;
        }
    }
}
