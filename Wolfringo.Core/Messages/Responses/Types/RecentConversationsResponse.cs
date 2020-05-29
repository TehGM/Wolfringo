using Newtonsoft.Json;
using System.Collections.Generic;

namespace TehGM.Wolfringo.Messages.Responses
{
    public class RecentConversationsResponse : ChatHistoryResponse, IWolfResponse
    {
        [JsonProperty("headers")]
        public IReadOnlyDictionary<string, object> Headers { get; private set; }
    }
}
