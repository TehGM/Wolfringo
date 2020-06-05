using Newtonsoft.Json;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>Group joining message.</summary>
    public class GroupJoinMessage : IWolfMessage
    {
        /// <inheritdoc/>
        [JsonIgnore]
        public string Command => MessageCommands.GroupMemberAdd;

        /// <summary>ID of the group.</summary>
        [JsonProperty("groupId")]
        public uint GroupID { get; private set; }

        // sending only
        /// <summary>Group password.</summary>
        [JsonProperty("password", NullValueHandling = NullValueHandling.Ignore)]
        public string Password { get; private set; }
        // receiving only
        /// <summary>User who joined the group.</summary>
        [JsonProperty("subscriberId", NullValueHandling = NullValueHandling.Ignore)]
        public uint? UserID { get; private set; }
        /// <summary>User permission level in the group.</summary>
        [JsonProperty("capabilities", NullValueHandling = NullValueHandling.Ignore)]
        public WolfGroupCapabilities? Capabilities { get; private set; }

        [JsonConstructor]
        private GroupJoinMessage() { }

        /// <summary>Creates a message instance.</summary>
        /// <param name="groupID">ID of the group to join.</param>
        /// <param name="password">Password to use when joining the group.</param>
        public GroupJoinMessage(uint groupID, string password = null) : this()
        {
            this.GroupID = groupID;
            this.Password = password ?? string.Empty;
        }
    }
}
