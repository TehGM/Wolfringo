using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using TehGM.Wolfringo.Messages.Responses;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>A message indicating chat message update.</summary>
    /// <remarks>Uses <see cref="ChatUpdateResponse"/> as response type.</remarks>
    [ResponseType(typeof(ChatUpdateResponse))]
    public class ChatUpdateMessage : IWolfMessage, IRawDataMessage
    {
        [JsonIgnore]
        public string Command => MessageCommands.MessageUpdate;

        // json data
        /// <summary>Is it a group message?</summary>
        [JsonProperty("isGroup")]
        public bool IsGroupMessage { get; protected set; }
        /// <summary>Message's timestamp. Used by protocol as message ID</summary>
        [JsonProperty("timestamp")]
        [JsonConverter(typeof(WolfTimestampConverter))]
        public DateTime Timestamp { get; protected set; }
        /// <summary>User that sent the message.</summary>
        [JsonProperty("originator", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(EntityIdConverter))]
        public uint? SenderID { get; protected set; }
        /// <summary>User or group that received the message.</summary>
        [JsonProperty("recipient")]
        [JsonConverter(typeof(EntityIdConverter))]
        public uint RecipientID { get; protected set; }
        /// <summary>Information about message's latest edit.</summary>
        [JsonProperty("edited", NullValueHandling = NullValueHandling.Ignore)]
        public ChatMessage.EditMetadata? EditInfo { get; protected set; }
        /// <summary>Is this message soft-deleted by group admin?</summary>
        [JsonProperty("isDeleted", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsDeleted { get; protected set; }
        /// <summary>Is this message tipped?</summary>
        [JsonProperty("isTipped", NullValueHandling = NullValueHandling.Ignore)]
        public bool? HasTips { get; private set; }

        // binary data
        /// <summary>Message's raw binary data.</summary>
        [JsonIgnore]
        public IReadOnlyCollection<byte> RawData { get; }

        /// <summary>Message's text.</summary>
        [JsonIgnore]
        public string Text => Encoding.UTF8.GetString(this.RawData.ToArray());

        [JsonConstructor]
        protected ChatUpdateMessage()
        {
            this.RawData = new List<byte>();
        }

        public class Builder
        {
            /// <summary>Message's timestamp. Used by protocol as message ID.</summary>
            public DateTime Timestamp { get; }
            /// <summary>User or group that received the message.</summary>
            public uint RecipientID { get; }
            /// <summary>Is it a group message?</summary>
            public bool IsGroupMessage { get; }

            /// <summary>Is this message soft-deleted by group admin?</summary>
            public bool? IsDeleted { get; set; }

            public Builder(IChatMessage message)
            {
                if (message.Timestamp == null)
                    throw new ArgumentException($"{nameof(ChatUpdateMessage)} can only be used for messages already processed by the Wolf server", nameof(message));
                this.Timestamp = message.Timestamp.Value;
                this.RecipientID = message.RecipientID;
                this.IsGroupMessage = message.IsGroupMessage;
            }

            public ChatUpdateMessage Build()
            {
                return new ChatUpdateMessage()
                {
                    Timestamp = this.Timestamp,
                    RecipientID = this.RecipientID,
                    IsGroupMessage = this.IsGroupMessage,

                    IsDeleted = this.IsDeleted
                };
            }
        }
    }
}
