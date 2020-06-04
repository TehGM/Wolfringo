using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TehGM.Wolfringo.Messages.Responses;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo.Messages
{
    [ResponseType(typeof(ChatHistoryResponse))]
    public class GroupChatHistoryMessage : IWolfMessage, IHeadersWolfMessage
    {
        [JsonIgnore]
        public string Command => MessageCommands.MessageGroupHistoryList;
        [JsonIgnore]
        public IDictionary<string, object> Headers { get; } = new Dictionary<string, object>()
        {
            { "version", 3 }
        };

        [JsonProperty("id")]
        public uint GroupID { get; private set; }
        [JsonProperty("timestampBegin")]
        [JsonConverter(typeof(MillisecondsEpochConverter))]
        public DateTime AfterTime { get; private set; }
        [JsonProperty("timestampEnd", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(MillisecondsEpochConverter))]
        public DateTime? BeforeTime { get; private set; }
        [JsonProperty("chronological")]
        public bool RequestChronologicalOrder { get; private set; }

        [JsonConstructor]
        private GroupChatHistoryMessage() { }

        public GroupChatHistoryMessage(uint groupId, DateTime? before, bool chronological = false)
        {
            this.GroupID = groupId;
            this.BeforeTime = before;
            this.AfterTime = MillisecondsEpochConverter.Epoch.AddMilliseconds(1);
            this.RequestChronologicalOrder = chronological;
        }
    }
}
