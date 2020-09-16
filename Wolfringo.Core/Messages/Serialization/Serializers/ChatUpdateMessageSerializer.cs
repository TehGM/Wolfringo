using System.Linq;
using Newtonsoft.Json.Linq;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo.Messages.Serialization
{
    /// <summary>Serializer for chat message edit message, such as message deletion.</summary>
    /// <remarks>This special serializer moves "metadata" properties into "metadata" object to match the protocol.</remarks>
    public class ChatUpdateMessageSerializer : DefaultMessageSerializer<ChatUpdateMessage>
    {
        /// <inheritdoc/>
        public override IWolfMessage Deserialize(string command, SerializedMessageData messageData)
        {
            // deserialize message
            ChatUpdateMessage result = (ChatUpdateMessage)base.Deserialize(command, messageData);

            // parse and populate binary data
            if (messageData.BinaryMessages?.Any() == true)
                SerializationHelper.PopulateMessageRawData(ref result, messageData.BinaryMessages.First());

            return result;
        }

        /// <inheritdoc/>
        public override SerializedMessageData Serialize(IWolfMessage message)
        {
            SerializedMessageData result = base.Serialize(message);
            JObject body = result.Payload["body"] as JObject;
            if (body == null)
                return result;
            // metadata props
            JObject metadata = new JObject();
            SerializationHelper.MovePropertyIfExists(ref body, ref metadata, "isDeleted");
            SerializationHelper.MovePropertyIfExists(ref body, ref metadata, "isTipped");
            if (metadata.HasValues)
                body.Add(new JProperty("metadata", metadata));

            // palringo inconsistency strikes again!!!
            // when receiving, recipient is in object as "recipient". But when sending, it's "recipientId". So here, for sending we rename it
            // new protocol feature, same damn bad design
            if (message is ChatUpdateMessage updateMessage && updateMessage.SenderID == null)
            {
                JToken recipient = body["recipient"];
                body.Remove("recipient");
                body.Add("recipientId", recipient);
            }
            return result;
        }
    }
}
