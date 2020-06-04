using Newtonsoft.Json;
using System;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo.Messages.Responses
{
    public class ChatResponse : WolfResponse, IWolfResponse
    {
        [JsonProperty("uuid")]
        public Guid ID { get; private set; }
        [JsonProperty("timestamp")]
        [JsonConverter(typeof(MillisecondsEpochConverter))]
        public DateTime Timestamp { get; private set; }
        [JsonProperty("isSpam")]
        public bool SpamFiltered { get; private set; }
    }
}
