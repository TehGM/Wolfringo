using Newtonsoft.Json.Linq;
using System.Linq;

namespace TehGM.Wolfringo.Messages.Serialization
{
    public class ChatMessageSerializer : JsonMessageSerializer<ChatMessage>
    {
        public override IWolfMessage Deserialize(string command, SerializedMessageData messageData)
        {
            // due to how stupid the protocol is, chat message needs unwrapping of body
            ChatMessage result = (ChatMessage)base.Deserialize(command, new SerializedMessageData(messageData.Payload["body"], messageData.BinaryMessages));

            // return null to cancel further execution if it's mime types that are more nicely sent in normal events
            if (result.Type == ChatMessageTypes.PrivateRequestResponse || result.Type == ChatMessageTypes.GroupAction)
                return null;

            // text comes with offset character \u0004, and we don't need it, so skip it
            result.RawData = messageData.BinaryMessages.First().Skip(1).ToArray();
            return result;
        }

        public override SerializedMessageData Serialize(IWolfMessage message)
        {
            JObject payload = GetJsonPayload(message);
            ChatMessage msg = (ChatMessage)message;
            if (msg.RawData != null)
                payload["body"]["data"] = new JObject(new JProperty("_placeholder", true), new JProperty("num", 0));
            return new SerializedMessageData(payload, ((ChatMessage)message).RawData);
        }
    }
}
