using Newtonsoft.Json;
using System.Collections.Generic;

namespace TehGM.Wolfringo.Messages
{
    public class RecentConversationsMessage : IWolfMessage, IHeadersWolfMessage
    {
        [JsonIgnore]
        public string Command => MessageCommands.MessageConversationList;
        [JsonIgnore]
        public IDictionary<string, object> Headers { get; } = new Dictionary<string, object>()
        {
            { "version", 3 }
        };
    }
}
