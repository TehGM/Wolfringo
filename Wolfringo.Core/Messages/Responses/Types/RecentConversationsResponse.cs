using Newtonsoft.Json;
using System.Collections.Generic;

namespace TehGM.Wolfringo.Messages.Responses
{
    /// <summary>Wolf server's response for <see cref="RecentConversationsMessage"/>.</summary>
    public class RecentConversationsResponse : ChatHistoryResponse, IWolfResponse
    {
        /// <summary>Response headers.</summary>
        [JsonProperty("headers")]
        public IReadOnlyDictionary<string, object> Headers { get; private set; }

        /// <summary>Creates a response instance.</summary>
        [JsonConstructor]
        protected RecentConversationsResponse() : base() { }
    }
}
