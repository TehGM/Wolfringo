using Newtonsoft.Json;
using System.Collections.Generic;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>A message for requesting group members list.</summary>
    /// <remarks>Uses <see cref="GroupMembersListResponse"/> as response type.</remarks>
    [ResponseType(typeof(GroupMembersListResponse))]
    public class GroupMembersListMessage : IWolfMessage, IHeadersWolfMessage
    {
        /// <inheritdoc/>
        /// <remarks>Equals to <see cref="MessageEventNames.GroupMemberList"/>.</remarks>
        [JsonIgnore]
        public string EventName => MessageEventNames.GroupMemberList;
        /// <inheritdoc/>
        [JsonIgnore]
        public IDictionary<string, object> Headers { get; } = new Dictionary<string, object>()
        {
            { "version", 3 }
        };

        /// <summary>ID of the group.</summary>
        [JsonProperty("id")]
        public uint GroupID { get; private set; }
        /// <summary>Subscribe to group members' profile updates?</summary>
        [JsonProperty("subscribe")]
        public bool SubscribeToUpdates { get; private set; }

        /// <summary>Creates a message instance.</summary>
        [JsonConstructor]
        protected GroupMembersListMessage() { }

        /// <summary>Creates a message instance.</summary>
        /// <param name="groupID">ID of the group to get members of.</param>
        /// <param name="subscribe">Subscribe to group members' profile updates?</param>
        public GroupMembersListMessage(uint groupID, bool subscribe = true)
            : this()
        {
            this.GroupID = groupID;
            this.SubscribeToUpdates = subscribe;
        }
    }
}
