using Newtonsoft.Json;
using System.Collections.Generic;

namespace TehGM.Wolfringo.Messages.Responses
{
    /// <summary>Wolf server's response for <see cref="CharmListMessage"/>.</summary>
    public class CharmListResponse : WolfResponse, IWolfResponse
    {
        /// <summary>Charms.</summary>
        [JsonProperty("body")]
        public IEnumerable<WolfCharm> Charms { get; private set; }

        [JsonConstructor]
        protected CharmListResponse() : base() { }
    }
}
