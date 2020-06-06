using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TehGM.Wolfringo.Messages.Responses;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>A message for requesting group chat history.</summary>
    /// <remarks>Uses <see cref="ChatHistoryResponse"/> as response type.</remarks>
    [ResponseType(typeof(ChatHistoryResponse))]
    public class GroupChatHistoryMessage : IWolfMessage, IHeadersWolfMessage
    {
        /// <inheritdoc/>
        [JsonIgnore]
        public string Command => MessageCommands.MessageGroupHistoryList;
        /// <inheritdoc/>
        [JsonIgnore]
        public IDictionary<string, object> Headers { get; } = new Dictionary<string, object>()
        {
            { "version", 3 }
        };

        /// <summary>ID of the group.</summary>
        [JsonProperty("id")]
        public uint GroupID { get; private set; }
        /// <summary>Timestamp of the oldest message to retrieve.</summary>
        [JsonProperty("timestampBegin")]
        [JsonConverter(typeof(MillisecondsEpochConverter))]
        public DateTime AfterTime { get; private set; }
        /// <summary>Timestamp of the oldest already received message.</summary>
        [JsonProperty("timestampEnd", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(MillisecondsEpochConverter))]
        public DateTime? BeforeTime { get; private set; }
        /// <summary>Should history be ordered chronologically?</summary>
        [JsonProperty("chronological")]
        public bool RequestChronologicalOrder { get; private set; }

        [JsonConstructor]
        protected GroupChatHistoryMessage() { }

        /// <summary>Creates a message instance.</summary>
        /// <param name="groupId">ID of the group.</param>
        /// <param name="before">Timestamp of the oldest already received message.</param>
        /// <param name="chronological">Should history be ordered chronologically?</param>
        public GroupChatHistoryMessage(uint groupId, DateTime? before, bool chronological = false)
        {
            this.GroupID = groupId;
            this.BeforeTime = before;
            this.AfterTime = MillisecondsEpochConverter.Epoch.AddMilliseconds(1);
            this.RequestChronologicalOrder = chronological;
        }
    }
}
