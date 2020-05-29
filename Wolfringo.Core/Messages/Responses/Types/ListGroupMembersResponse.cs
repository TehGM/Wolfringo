using Newtonsoft.Json;
using System.Collections.Generic;

namespace TehGM.Wolfringo.Messages.Responses
{
    public class ListGroupMembersResponse : WolfResponse, IWolfResponse
    {
        [JsonProperty("body")]
        public IEnumerable<WolfGroupMember> GroupMembers { get; private set; }
    }
}
