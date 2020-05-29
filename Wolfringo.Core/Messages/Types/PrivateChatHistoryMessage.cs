using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TehGM.Wolfringo.Messages.Responses;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo.Messages
{
    [ResponseType(typeof(ChatHistoryResponse))]
    public class PrivateChatHistoryMessage : IWolfMessage, IHeadersWolfMessage
    {
        [JsonIgnore]
        public string Command => MessageCommands.MessagePrivateHistoryList;
        [JsonIgnore]
        public IDictionary<string, object> Headers { get; } = new Dictionary<string, object>()
        {
            { "version", 2 }
        };

        [JsonProperty("id")]
        public uint UserID { get; private set; }
        [JsonProperty("timestampEnd", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(EpochConverter))]
        public DateTime? BeforeTime { get; private set; }

        [JsonConstructor]
        private PrivateChatHistoryMessage() { }

        public PrivateChatHistoryMessage(uint userID, DateTime? before)
        {
            this.UserID = userID;
            this.BeforeTime = before;
        }
    }
}
