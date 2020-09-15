using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo.Messages.Responses
{
    public class EntitiesSubscribeResponse : WolfResponse, IWolfResponse
    {
        [JsonProperty("body")]
        [JsonConverter(typeof(KeyAndValueDictionaryConverter<uint, HttpStatusCode>), "code")]
        public IReadOnlyDictionary<uint, HttpStatusCode> Results { get; private set; }

        [JsonConstructor]
        protected EntitiesSubscribeResponse() : base() { }
    }
}
