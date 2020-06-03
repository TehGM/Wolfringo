using Newtonsoft.Json;
using System.Collections.Generic;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo.Messages.Responses
{
    public class BlockListResponse : WolfResponse, IWolfResponse
    {
        [JsonProperty("body", ItemConverterType = typeof(EntityIdConverter))]
        public IEnumerable<uint> BlockedUsersIDs { get; private set; }
    }
}
