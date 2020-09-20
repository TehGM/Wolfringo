using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>A message for requesting details on message's tips.</summary>
    /// <remarks>Uses <see cref="TipDetailsResponse"/> as response type.</remarks>
    [ResponseType(typeof(TipDetailsResponse))]
    public class TipDetailsMessage : IWolfMessage
    {
        [JsonIgnore]
        public string Command => MessageCommands.TipDetail;

        /// <summary>ID (timestamp) of the message to get tip details of.</summary>
        [JsonProperty("id")]
        public WolfTimestamp MessageID { get; private set; }
        /// <summary>ID of the group where the message is in.</summary>
        [JsonProperty("groupId")]
        public uint GroupID { get; private set; }
        /// <summary>Context type of the tip.</summary>
        [JsonProperty("contextType")]
        [JsonConverter(typeof(StringEnumConverter), true)]
        public WolfTip.ContextType ContextType { get; private set; }

        [JsonConstructor]
        protected TipDetailsMessage() { }

        /// <summary>Creates a message instance.</summary>
        /// <param name="messageID">ID (timestamp) of the message to tip.</param>
        /// <param name="groupID">ID of the group where the message is in.</param>
        /// <param name="contextType">Context type of the tip.</param>
        public TipDetailsMessage(WolfTimestamp messageID, uint groupID, WolfTip.ContextType contextType)
        {
            this.GroupID = groupID;
            this.ContextType = contextType;
            this.MessageID = messageID;
        }
    }
}
