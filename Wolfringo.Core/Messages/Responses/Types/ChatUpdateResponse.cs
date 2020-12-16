using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo.Messages.Responses
{
    /// <summary>Wolf server's response for <see cref="ChatUpdateMessage"/>.</summary>
    public class ChatUpdateResponse : WolfResponse, IRawDataMessage
    {
        // json data
        /// <summary>Is it a group message?</summary>
        [JsonProperty("isGroup")]
        public bool IsGroupMessage { get; private set; }
        /// <summary>Type of the message.</summary>
        [JsonProperty("mimeType")]
        public string MimeType { get; private set; }
        /// <summary>Message's timestamp.</summary>
        [JsonProperty("timestamp")]
        public WolfTimestamp Timestamp { get; private set; }
        /// <summary>User that sent the message.</summary>
        [JsonProperty("originator", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(EntityIdConverter))]
        public uint SenderID { get; private set; }
        /// <summary>User or group that received the message.</summary>
        [JsonProperty("recipient")]
        [JsonConverter(typeof(EntityIdConverter))]
        public uint RecipientID { get; private set; }
        /// <summary>Information about message's latest edit.</summary>
        [JsonProperty("edited")]
        public ChatMessage.EditMetadata? EditInfo { get; private set; }
        /// <summary>Is this message soft-deleted by group admin?</summary>
        [JsonProperty("isDeleted", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public bool IsDeleted { get; private set; }

        // binary data
        /// <inheritdoc/>
        [JsonIgnore]
        public IReadOnlyCollection<byte> RawData { get; private set; }

        // helper props
        /// <summary>Is it a private message?</summary>
        [JsonIgnore]
        public bool IsPrivateMessage => !this.IsGroupMessage;
        /// <summary>Message's text.</summary>
        [JsonIgnore]
        public string Text => Encoding.UTF8.GetString(this.RawData.ToArray());
        /// <summary>Is it a text message?</summary>
        [JsonIgnore]
        public bool IsText => this.MimeType == ChatMessageTypes.Text;
        /// <summary>Is it an image message?</summary>
        [JsonIgnore]
        public bool IsImage => this.MimeType == ChatMessageTypes.ImageLink || this.MimeType == ChatMessageTypes.Image;
        /// <summary>Is it a voice message?</summary>
        [JsonIgnore]
        public bool IsVoice => this.MimeType == ChatMessageTypes.VoiceLink || this.MimeType == ChatMessageTypes.Voice;

        /// <summary>Creates a response instance.</summary>
        [JsonConstructor]
        protected ChatUpdateResponse() : base()
        {
            this.RawData = new List<byte>();
        }
    }
}
