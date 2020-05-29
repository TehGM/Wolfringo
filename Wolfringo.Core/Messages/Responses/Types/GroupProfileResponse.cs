using Newtonsoft.Json;
using System.Collections.Generic;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo.Messages.Responses
{
    public class GroupProfileResponse : WolfResponse, IWolfResponse
    {
        [JsonProperty("body")]
        [JsonConverter(typeof(ExtractValuesOnlyConverter<WolfGroup>))]
        public IEnumerable<WolfGroup> GroupProfiles { get; private set; }
    }
}
