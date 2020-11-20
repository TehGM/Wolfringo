using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>A message for requesting summary on messages tips.</summary>
    /// <remarks>Uses <see cref="TipSummaryResponse"/> as response type.</remarks>
    [ResponseType(typeof(TipSummaryResponse))]
    public class TipSummaryMessage : IWolfMessage
    {
        [JsonIgnore]
        public string EventName => MessageEventNames.TipSummary;

        /// <summary>Request context type.</summary>
        [JsonProperty("contextType")]
        [JsonConverter(typeof(StringEnumConverter), true)]
        public WolfTip.ContextType ContextType { get; private set; }
        /// <summary>Group where the messages are in.</summary>
        [JsonProperty("groupId")]
        public uint GroupID { get; private set; }
        /// <summary>List of message IDs (timestamps) to request summary of.</summary>
        [JsonProperty("idList")]
        public IEnumerable<WolfTimestamp> MessageIDs { get; private set; }

        /// <summary>Creates a message instance.</summary>
        /// <param name="contextType">Request context type.</param>
        /// <param name="groupID">Group where the messages are in.</param>
        /// <param name="messageIDs">List of message IDs (timestamps) to request summary of.</param>
        public TipSummaryMessage(WolfTip.ContextType contextType, uint groupID, IEnumerable<WolfTimestamp> messageIDs)
        {
            if (messageIDs?.Any() != true)
                throw new ArgumentException("Must request at least one message ID", nameof(messageIDs));
            this.ContextType = contextType;
            this.MessageIDs = new ReadOnlyCollection<WolfTimestamp>((messageIDs as IList<WolfTimestamp>) ?? messageIDs.ToArray());
            this.GroupID = groupID;
        }
    }
}
