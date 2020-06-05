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
    public class ChatMessage : IChatMessage, IWolfMessage
    {
        /// <inheritdoc/>
        [JsonIgnore]
        public string Command => MessageCommands.MessageSend;

        // json data
        /// <inheritdoc/>
        public string FlightID { get; protected set; }
        /// <inheritdoc/>
        public Guid? ID { get; protected set; }
        /// <inheritdoc/>
        public bool IsGroupMessage { get; protected set; }
        /// <inheritdoc/>
        public string MimeType { get; protected set; }
        /// <inheritdoc/>
        public DateTime? Timestamp { get; protected set; }
        /// <inheritdoc/>
        public uint? SenderID { get; protected set; }
        /// <inheritdoc/>
        public uint RecipientID { get; private set; }

        // binary data
        /// <inheritdoc/>
        [JsonIgnore]
        public IReadOnlyCollection<byte> RawData { get; protected set; }

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
        public ChatMessage(uint recipientID, bool groupMessage, string type, IEnumerable<byte> data) : this()
        {
            this.RecipientID = recipientID;
            this.MimeType = type;
            this.IsGroupMessage = groupMessage;
            this.FlightID = Guid.NewGuid().ToString();
            this.RawData = (data as IReadOnlyCollection<byte>) ?? new List<byte>(data);
        }
    }
}
