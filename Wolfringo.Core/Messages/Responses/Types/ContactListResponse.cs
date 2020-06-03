using Newtonsoft.Json;
using System.Collections.Generic;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo.Messages.Responses
{
    public class ContactListResponse : WolfResponse, IWolfResponse
    {
        [JsonProperty("body", ItemConverterType = typeof(EntityIdConverter))]
        public IEnumerable<uint> ContactIDs { get; private set; }
    }
}
