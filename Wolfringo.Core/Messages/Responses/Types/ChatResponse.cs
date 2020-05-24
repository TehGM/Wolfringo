using Newtonsoft.Json;
using System;

namespace TehGM.Wolfringo.Messages.Responses
{
    public class ChatResponse : WolfResponse, IWolfResponse
    {
        [JsonProperty("uuid")]
        public Guid ID { get; private set; }
        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; private set; }
        [JsonProperty("isSpam")]
        public bool SpamFiltered { get; private set; }
    }
}
