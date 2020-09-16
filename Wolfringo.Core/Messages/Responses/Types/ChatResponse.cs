using Newtonsoft.Json;
using System;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo.Messages.Responses
{
    /// <summary>Wolf server's response for <see cref="ChatMessage"/>.</summary>
    public class ChatResponse : WolfResponse, IWolfResponse
    {
        /// <summary>Unique ID of the message.</summary>
        [JsonProperty("uuid")]
        public Guid ID { get; private set; }
        /// <summary>Timestamp of the message.</summary>
        [JsonProperty("timestamp")]
        [JsonConverter(typeof(WolfTimestampConverter))]
        public DateTime Timestamp { get; private set; }
        /// <summary>Was the message spam filtered?</summary>
        [JsonProperty("isSpam")]
        public bool SpamFiltered { get; private set; }

        [JsonConstructor]
        protected ChatResponse() : base() { }
    }
}
