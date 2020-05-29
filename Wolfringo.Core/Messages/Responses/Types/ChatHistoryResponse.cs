using Newtonsoft.Json;
using System.Collections.Generic;

namespace TehGM.Wolfringo.Messages.Responses
{
    public class ChatHistoryResponse : WolfResponse, IWolfResponse
    {
        [JsonProperty("body")]
        public IEnumerable<ChatMessage> Messages { get; private set; }
    }
}
