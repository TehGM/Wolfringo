using Newtonsoft.Json;
using System;

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
        public WolfTimestamp Timestamp { get; private set; }
        /// <summary>Was the message spam filtered?</summary>
        [JsonProperty("isSpam")]
        public bool SpamFiltered { get; private set; }

        /// <summary>Creates a response instance.</summary>
        [JsonConstructor]
        protected ChatResponse() : base() { }
    }
}
