using Newtonsoft.Json;
using System.Collections.Generic;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo.Messages
{
    [ResponseType(typeof(ListGroupMembersResponse))]
    public class ListGroupMembersMessage : IWolfMessage, IHeadersWolfMessage
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
        private ListGroupMembersMessage() { }

        public ListGroupMembersMessage(uint groupID, bool subscribe = true)
            : this()
        {
            this.GroupID = groupID;
            this.SubscribeToUpdates = subscribe;
        }
    }
}
