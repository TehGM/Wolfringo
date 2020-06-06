using Newtonsoft.Json;
using System.Collections.Generic;

namespace TehGM.Wolfringo.Messages.Responses
{
    /// <summary>Wolf server's response for <see cref="GroupMembersListMessage"/>.</summary>
    public class GroupMembersListResponse : WolfResponse, IWolfResponse
    {
        /// <summary>Group members.</summary>
        [JsonProperty("body")]
        public IEnumerable<WolfGroupMember> GroupMembers { get; private set; }

        [JsonConstructor]
        protected GroupMembersListResponse() : base() { }
    }
}
