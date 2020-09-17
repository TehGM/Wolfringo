using System.Collections.Generic;
using Newtonsoft.Json;

namespace TehGM.Wolfringo.Messages.Responses
{
    /// <summary>Represents a response for message tips details request.</summary>
    public class TipDetailsResponse : WolfResponse, IWolfResponse
    {
        /// <summary>Tag for the response.</summary>
        [JsonProperty("etag")]
        public string Etag { get; private set; }

        /// <summary>ID (timestamp) of the message.</summary>
        [JsonProperty("id")]
        public WolfTimestamp Timestamp { get; private set; }
        /// <summary>Details on message tips.</summary>
        [JsonProperty("list")]
        public IEnumerable<WolfTip> Tips { get; private set; }
        [JsonProperty("version")]
        public int Version { get; private set; }

        [JsonConstructor]
        protected TipDetailsResponse() { }
    }
}
