using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo.Messages
{
    [ResponseType(typeof(ChatResponse))]
    public class ChatMessage : IChatMessage, IWolfMessage
    {
        public string Command => MessageCommands.MessageSend;

        // json data
        public string FlightID { get; protected set; }
        public Guid? ID { get; protected set; }
        public bool IsGroupMessage { get; protected set; }
        public string MimeType { get; protected set; }
        public DateTime? Timestamp { get; protected set; }
        public uint? SenderID { get; protected set; }
        public uint RecipientID { get; private set; }

        // binary data
        [JsonIgnore]
        public IReadOnlyCollection<byte> RawData { get; protected set; }

        // helper props 
        [JsonIgnore]
        public bool IsPrivateMessage => !this.IsGroupMessage;
        [JsonIgnore]
        public string Text => Encoding.UTF8.GetString(this.RawData.ToArray());
        [JsonIgnore]
        public bool IsText => this.MimeType == ChatMessageTypes.Text;
        [JsonIgnore]
        public bool IsImage => this.MimeType == ChatMessageTypes.ImageLink || this.MimeType == ChatMessageTypes.Image;
        [JsonIgnore]
        public bool IsVoice => this.MimeType == ChatMessageTypes.VoiceLink || this.MimeType == ChatMessageTypes.Voice;

        [JsonConstructor]
        protected ChatMessage()
        {
            this.RawData = new List<byte>();
        }

        public ChatMessage(uint recipientID, bool groupMessage, string type, IEnumerable<byte> data) : this()
        {
            this.RecipientID = recipientID;
            this.MimeType = type;
            this.IsGroupMessage = groupMessage;
            this.FlightID = Guid.NewGuid().ToString();
            this.RawData = (data as IReadOnlyCollection<byte>) ?? new List<byte>(data);
        }

        // helper create static methods
        public static ChatMessage TextMessage(uint recipientID, bool groupMessage, string text)
            => new ChatMessage(recipientID, groupMessage, ChatMessageTypes.Text, Encoding.UTF8.GetBytes(text));
        public static ChatMessage ImageMessage(uint recipientID, bool groupMessage, IEnumerable<byte> imageData)
            => new ChatMessage(recipientID, groupMessage, ChatMessageTypes.Image, imageData);
        public static ChatMessage VoiceMessage(uint recipientID, bool groupMessage, IEnumerable<byte> voiceData)
            => new ChatMessage(recipientID, groupMessage, ChatMessageTypes.Voice, voiceData);
    }
}
