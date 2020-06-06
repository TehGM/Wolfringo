using Newtonsoft.Json;
using System.Collections.Generic;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo.Messages.Responses
{
    /// <summary>Wolf server's response for <see cref="GroupChatHistoryMessage"/> and <see cref="PrivateChatHistoryMessage"/>.</summary>
    public class ChatHistoryResponse : WolfResponse, IWolfResponse
    {
        /// <summary>Messages from history.</summary>
        [JsonProperty("body", ItemConverterType = typeof(ChatMessageConverter))]
        public IEnumerable<IChatMessage> Messages { get; private set; }

        [JsonConstructor]
        protected ChatHistoryResponse() : base() { }
    }
}
