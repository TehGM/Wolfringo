using Newtonsoft.Json;
using System.Collections.Generic;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>A message for requesting private chat history.</summary>
    /// <remarks>Uses <see cref="ChatHistoryResponse"/> as response type.</remarks>
    [ResponseType(typeof(ChatHistoryResponse))]
    public class PrivateChatHistoryMessage : IWolfMessage, IHeadersWolfMessage
    {
        /// <inheritdoc/>
        [JsonIgnore]
        public string EventName => MessageEventNames.MessagePrivateHistoryList;
        /// <inheritdoc/>
        [JsonIgnore]
        public IDictionary<string, object> Headers { get; } = new Dictionary<string, object>()
        {
            { "version", 2 }
        };

        /// <summary>ID of the user.</summary>
        [JsonProperty("id")]
        public uint UserID { get; private set; }
        /// <summary>Timestamp of the oldest already received message.</summary>
        [JsonProperty("timestampEnd", NullValueHandling = NullValueHandling.Ignore)]
        public WolfTimestamp? BeforeTime { get; private set; }

        [JsonConstructor]
        protected PrivateChatHistoryMessage() { }

        /// <summary>Creates a message instance.</summary>
        /// <param name="userID">ID of the user.</param>
        /// <param name="before">Timestamp of the oldest already received message.</param>
        public PrivateChatHistoryMessage(uint userID, WolfTimestamp? before)
        {
            this.UserID = userID;
            this.BeforeTime = before;
        }
    }
}
