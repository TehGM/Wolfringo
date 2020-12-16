﻿using Newtonsoft.Json;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>Group joining message.</summary>
    public class GroupJoinMessage : IWolfMessage
    {
        /// <inheritdoc/>
        /// <remarks>Equals to <see cref="MessageEventNames.GroupMemberAdd"/>.</remarks>
        [JsonIgnore]
        public string EventName => MessageEventNames.GroupMemberAdd;

        /// <summary>ID of the group.</summary>
        [JsonProperty("groupId", NullValueHandling = NullValueHandling.Ignore)]
        public uint? GroupID { get; private set; }
        /// <summary>Name of the group.</summary>
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string GroupName { get; private set; }

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

        /// <summary>Creates a message instance.</summary>
        [JsonConstructor]
        protected GroupJoinMessage() { }

        /// <summary>Creates a message instance.</summary>
        /// <param name="groupID">ID of the group to join.</param>
        /// <param name="password">Password to use when joining the group.</param>
        public GroupJoinMessage(uint groupID, string password = null) : this()
        {
            this.GroupID = groupID;
            this.GroupName = null;
            this.Password = password ?? string.Empty;
        }

        /// <summary>Creates a message instance.</summary>
        /// <param name="groupName">Name of the group to join.</param>
        /// <param name="password">Password to use when joining the group.</param>
        public GroupJoinMessage(string groupName, string password = null) : this()
        {
            this.GroupID = null;
            this.GroupName = groupName;
            this.Password = password ?? string.Empty;
        }
    }
}
