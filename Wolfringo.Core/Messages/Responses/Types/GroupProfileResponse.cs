using Newtonsoft.Json;
using System.Collections.Generic;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo.Messages.Responses
{
    /// <summary>Wolf server's response for <see cref="GroupProfileMessage"/>.</summary>
    public class GroupProfileResponse : WolfResponse, IWolfResponse
    {
        /// <summary>Group profiles.</summary>
        [JsonProperty("body")]
        [JsonConverter(typeof(ExtractValuesOnlyConverter<WolfGroup>))]
        public IEnumerable<WolfGroup> GroupProfiles { get; private set; }

        [JsonConstructor]
        protected GroupProfileResponse() : base() { }
    }
}
