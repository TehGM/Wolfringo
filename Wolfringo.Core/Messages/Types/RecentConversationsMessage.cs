using Newtonsoft.Json;
using System.Collections.Generic;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>A message for requesting recent conversations' messages.</summary>
    /// <remarks>Uses <see cref="RecentConversationsResponse"/> as response type.</remarks>
    [ResponseType(typeof(RecentConversationsResponse))]
    public class RecentConversationsMessage : IWolfMessage, IHeadersWolfMessage
    {
        /// <inheritdoc/>
        [JsonIgnore]
        public string Command => MessageCommands.MessageConversationList;
        /// <inheritdoc/>
        [JsonIgnore]
        public IDictionary<string, object> Headers { get; } = new Dictionary<string, object>()
        {
            { "version", 3 }
        };
    }
}
