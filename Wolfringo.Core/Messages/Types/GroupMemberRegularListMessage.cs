using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>A message for requesting list of regular group members.</summary>
    /// <remarks>Uses <see cref="GroupMembersListResponse"/> as response type.</remarks>
    [ResponseType(typeof(GroupMembersListResponse))]
    public class GroupMemberRegularListMessage : IWolfMessage
    {
        /// <inheritdoc/>
        /// <remarks>Equals to <see cref="MessageEventNames.GroupMemberPrivilegedList"/>.</remarks>
        [JsonIgnore]
        public string EventName => MessageEventNames.GroupMemberRegularList;

        /// <summary>ID of the group.</summary>
        [JsonProperty("id")]
        public uint GroupID { get; private set; }
        /// <summary>Subscribe to group members' profile updates?</summary>
        [JsonProperty("subscribe")]
        public bool SubscribeToUpdates { get; private set; }

        /// <summary>How many members to skip (for pagination purposes).</summary>
        /// <seealso cref="Limit"/>
        [JsonProperty("after")]
        public uint Skip { get; private set; }
        /// <summary>How many members to retrieve (for pagination purposes).</summary>
        /// <seealso cref="Skip"/>
        [JsonProperty("limit")]
        public uint Limit { get; private set; }

        /// <summary>Creates a message instance.</summary>
        [JsonConstructor]
        protected GroupMemberRegularListMessage() { }

        /// <summary>Creates a message instance.</summary>
        /// <param name="groupID">ID of the group to get members of.</param>
        /// <param name="skip">How many members to skip (for pagination purposes).</param>
        /// <param name="limit">How many members to retrieve (for pagination purposes).</param>
        /// <param name="subscribe">Subscribe to group members' profile updates?</param>
        public GroupMemberRegularListMessage(uint groupID, uint skip, uint limit = 100, bool subscribe = true)
            : this()
        {
            this.GroupID = groupID;
            this.SubscribeToUpdates = subscribe;

            this.Skip = skip;
            this.Limit = limit;
        }
    }
}
