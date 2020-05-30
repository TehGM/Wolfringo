using Newtonsoft.Json;
using System.Collections.Generic;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo.Messages.Responses
{
    public class ChatHistoryResponse : WolfResponse, IWolfResponse
    {
        [JsonProperty("body", ItemConverterType = typeof(ChatMessageConverter))]
        public IEnumerable<IChatMessage> Messages { get; private set; }
    }
}
