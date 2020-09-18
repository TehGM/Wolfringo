using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo.Messages.Responses
{
    public class EntitiesSubscribeResponse : WolfResponse, IWolfResponse
    {
        /// <summary>Dictionary of IDs of entities that were attempted to subscribe, and the status code whether the subscribing was a success.</summary>
        [JsonProperty("body")]
        [JsonConverter(typeof(KeyAndValueDictionaryConverter<uint, HttpStatusCode>), "code")]
        public IReadOnlyDictionary<uint, HttpStatusCode> Results { get; private set; }

        [JsonConstructor]
        protected EntitiesSubscribeResponse() : base() { }
    }
}
