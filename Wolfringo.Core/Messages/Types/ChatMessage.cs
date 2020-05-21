using Newtonsoft.Json;
using System;
using System.Text;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo.Messages
{
    public class ChatMessage : IWolfMessage
    {
        public string Command => MessageCommands.Chat;

        // json data
        [JsonProperty("flightId", NullValueHandling = NullValueHandling.Ignore)]
        public Guid? FlightID { get; private set; }
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public Guid? ID { get; private set; }
        [JsonProperty("isGroup", Required = Required.Always)]
        public bool IsGroupMessage { get; private set; }
        [JsonProperty("mimeType", Required = Required.Always)]
        public string Type { get; private set; }
        [JsonProperty("timestamp", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? Timestamp { get; private set; }
        [JsonProperty("originator", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(UserIdConverter))]
        public uint? SenderID { get; private set; }
        [JsonProperty("recipient", Required = Required.Always)]
        [JsonConverter(typeof(UserIdConverter))]
        public uint RecipientID { get; private set; }

        // binary data
        [JsonIgnore]
        public byte[] RawData { get; set; }

        // helper props
        [JsonIgnore]
        public string Text => Encoding.UTF8.GetString(RawData);
        [JsonIgnore]
        public bool IsText => this.Type == ChatMessageTypes.Text;
        [JsonIgnore]
        public bool IsImage => this.Type == ChatMessageTypes.ImageLink || this.Type == ChatMessageTypes.Image;
        [JsonIgnore]
        public bool IsVoice => this.Type == ChatMessageTypes.VoiceLink || this.Type == ChatMessageTypes.Voice;
        [JsonIgnore]
        public bool IsPrivateMessage => !this.IsGroupMessage;

        // cosntructors
        [JsonConstructor]
        private ChatMessage() { }
        public ChatMessage(uint recipientID, bool groupMessage, string type, byte[] data)
            : this()
        {
            this.RecipientID = recipientID;
            this.Type = type;
            this.RawData = data;
            this.IsGroupMessage = groupMessage;
            this.FlightID = Guid.NewGuid();
        }

        // helper create static methods
        public static ChatMessage TextMessage(uint recipientID, bool groupMessage, string text)
            => new ChatMessage(recipientID, groupMessage, ChatMessageTypes.Text, Encoding.UTF8.GetBytes(text));
        public static ChatMessage ImageMessage(uint recipientID, bool groupMessage, byte[] imageData)
            => new ChatMessage(recipientID, groupMessage, ChatMessageTypes.Image, imageData);
        public static ChatMessage VoiceMessage(uint recipientID, bool groupMessage, byte[] voiceData)
            => new ChatMessage(recipientID, groupMessage, ChatMessageTypes.Voice, voiceData);
    }
}
