using Newtonsoft.Json;
using System.Collections.Generic;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo.Messages
{
    [ResponseType(typeof(GroupMembersListResponse))]
    public class GroupMemberListMessage : IWolfMessage, IHeadersWolfMessage
    {
        [JsonIgnore]
        public string Command => MessageCommands.GroupMemberList;
        [JsonIgnore]
        public IDictionary<string, object> Headers { get; } = new Dictionary<string, object>()
        {
            { "version", 3 }
        };

        [JsonProperty("id")]
        public uint GroupID { get; private set; }
        [JsonProperty("subscribe")]
        public bool SubscribeToUpdates { get; private set; }

        [JsonConstructor]
        private GroupMemberListMessage() { }

        public GroupMemberListMessage(uint groupID, bool subscribe = true)
            : this()
        {
            this.GroupID = groupID;
            this.SubscribeToUpdates = subscribe;
        }
    }
}
