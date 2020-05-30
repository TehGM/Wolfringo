using Newtonsoft.Json;
using System.Collections.Generic;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo.Messages.Responses
{
    public class ListUserGroupsResponse : WolfResponse, IWolfResponse
    {
        [JsonProperty("body", ItemConverterType = typeof(EntityIdConverter))]
        public IEnumerable<uint> UserGroupIDs { get; private set; }
    }
}
