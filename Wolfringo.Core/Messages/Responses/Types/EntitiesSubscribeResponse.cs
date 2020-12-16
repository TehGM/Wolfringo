using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo.Messages.Responses
{
    /// <summary>Wolf server's response for <see cref="SubscribeToGroupMessage"/> and <see cref="SubscribeToGroupTipsMessage"/>.</summary>
    public class EntitiesSubscribeResponse : WolfResponse, IWolfResponse
    {
        /// <summary>Dictionary of IDs of entities that were attempted to subscribe, and the status code whether the subscribing was a success.</summary>
        [JsonProperty("body")]
        [JsonConverter(typeof(KeyAndValueDictionaryConverter<uint, HttpStatusCode>), "code")]
        public IReadOnlyDictionary<uint, HttpStatusCode> Results { get; private set; }

        /// <summary>Creates a response instance.</summary>
        [JsonConstructor]
        protected EntitiesSubscribeResponse() : base() { }
    }
}
