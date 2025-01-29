using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>A normal chat message.</summary>
    /// <remarks>Uses <see cref="ChatResponse"/> as response type.</remarks>
    /// <seealso cref="GroupActionChatEvent"/>
    [ResponseType(typeof(ChatResponse))]
    public class ChatMessage : IChatMessage, IWolfMessage, IRawDataMessage
    {
        /// <inheritdoc/>
        /// <remarks>Equals to <see cref="MessageEventNames.MessageSend"/>.</remarks>
        [JsonIgnore]
        public string EventName => MessageEventNames.MessageSend;

        // json data
        /// <inheritdoc/>
        public string FlightID { get; private set; }
        /// <inheritdoc/>
        public Guid? ID { get; private set; }
        /// <inheritdoc/>
        public bool IsGroupMessage { get; private set; }
        /// <inheritdoc/>
        public string MimeType { get; private set; }
        /// <inheritdoc/>
        public WolfTimestamp? Timestamp { get; private set; }
        /// <inheritdoc/>
        public uint? SenderID { get; private set; }
        /// <inheritdoc/>
        public uint RecipientID { get; private set; }
        /// <summary>Information about message's latest edit.</summary>
        [JsonProperty("edited", NullValueHandling = NullValueHandling.Ignore)]
        public EditMetadata? EditInfo { get; private set; }
        /// <summary>Is this message soft-deleted by group admin?</summary>
        [JsonProperty("isDeleted", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public bool IsDeleted { get; private set; }
        /// <summary>Is this message tipped?</summary>
        [JsonProperty("isTipped", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public bool HasTips { get; private set; }
        /// <summary>The metadata for formatting of links in the message text.</summary>
        [JsonProperty("formatting", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public ChatMessageFormatting FormattingMetadata { get; private set; }
        /// <summary>Visual embeds attached to this chat message.</summary>
        [JsonProperty("embeds", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<IChatEmbed> Embeds { get; private set; }

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

        /// <summary>Creates a message instance.</summary>
        [JsonConstructor]
        protected ChatMessage()
        {
            this.RawData = new List<byte>();
        }

        /// <summary>Creates a message instance.</summary>
        /// <param name="recipientID">User or group ID to send the message to.</param>
        /// <param name="groupMessage">Is recipient a group?</param>
        /// <param name="type">Mime type of the message.</param>
        /// <param name="data">Raw byte data of the message.</param>
        public ChatMessage(uint recipientID, bool groupMessage, string type, IEnumerable<byte> data)
            : this(recipientID, groupMessage, type, data, null) { }

        /// <summary>Creates a message instance.</summary>
        /// <param name="recipientID">User or group ID to send the message to.</param>
        /// <param name="groupMessage">Is recipient a group?</param>
        /// <param name="type">Mime type of the message.</param>
        /// <param name="data">Raw byte data of the message.</param>
        /// <param name="formattingMetadata">Metadata for message formatting, such as group links.</param>
        public ChatMessage(uint recipientID, bool groupMessage, string type, IEnumerable<byte> data, ChatMessageFormatting formattingMetadata)
            : this(recipientID, groupMessage, type, data, formattingMetadata, null) { }


        /// <summary>Creates a message instance.</summary>
        /// <param name="recipientID">User or group ID to send the message to.</param>
        /// <param name="groupMessage">Is recipient a group?</param>
        /// <param name="type">Mime type of the message.</param>
        /// <param name="data">Raw byte data of the message.</param>
        /// <param name="formattingMetadata">Metadata for message formatting, such as group links.</param>
        /// <param name="embeds">Visual embeds attached to this chat message.</param>
        public ChatMessage(uint recipientID, bool groupMessage, string type, IEnumerable<byte> data, ChatMessageFormatting formattingMetadata, IEnumerable<IChatEmbed> embeds) : this()
        {
            this.RecipientID = recipientID;
            this.MimeType = type;
            this.IsGroupMessage = groupMessage;
            this.FlightID = Guid.NewGuid().ToString();
            this.FormattingMetadata = formattingMetadata;
            this.Embeds = embeds ?? Enumerable.Empty<IChatEmbed>();
            this.RawData = (data as IReadOnlyCollection<byte>) ?? new List<byte>(data);
        }


        /// <summary>Represents metadata about chat message edit.</summary>
        public struct EditMetadata
        {
            /// <summary>User that edited the message.</summary>
            [JsonProperty("subscriberId")]
            public uint UserID { get; private set; }
            /// <summary>Timestamp of message edit.</summary>
            [JsonProperty("timestamp")]
            public WolfTimestamp Timestamp { get; private set; }
        }
    }
}
