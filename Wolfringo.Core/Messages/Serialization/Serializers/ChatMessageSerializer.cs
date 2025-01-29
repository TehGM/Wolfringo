using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo.Messages.Serialization
{
    /// <summary>Serializer for chat messages with binary data.</summary>
    /// <remarks>This special serializer populates chat message's binary data collection when serializing. When deserializing,
    /// it inserts data placeholders into payload body object, and adds it to serialized message's binary data.</remarks>
    public class ChatMessageSerializer : IMessageSerializer
    {
        /// <inheritdoc/>
        public IWolfMessage Deserialize(string eventName, SerializedMessageData messageData)
        {
            // deserialize message
            Type msgType = GetMessageType(messageData.Payload["body"]);
            IChatMessage result = (IChatMessage)messageData.Payload.ToObject(msgType, SerializationHelper.DefaultSerializer);
            messageData.Payload.FlattenCommonProperties(result, SerializationHelper.DefaultSerializer);

            // parse and populate binary data
            if (messageData.BinaryMessages?.Any() == true)
                SerializationHelper.PopulateMessageRawData(ref result, messageData.BinaryMessages.First());

            return result;
        }

        /// <inheritdoc/>
        public SerializedMessageData Serialize(IWolfMessage message)
        {
            JObject payload = message.SerializeJsonPayload();
            JObject body = payload["body"] as JObject;
            IChatMessage msg = (IChatMessage)message;

            // metadata props
            JObject metadata = new JObject();
            SerializationHelper.MovePropertyIfExists(ref body, ref metadata, "isDeleted");
            SerializationHelper.MovePropertyIfExists(ref body, ref metadata, "isTipped");
            if (body.ContainsKey("formatting"))
            {
                JObject formatting = body["formatting"] as JObject;
                if (formatting == null)
                    body.Remove("formatting");

                if (formatting.ContainsKey("links"))
                {
                    JArray formattingLinks = formatting["links"] as JArray;
                    if (formattingLinks == null || !formattingLinks.Any())
                        formatting.Remove("links");
                }
                if (formatting.ContainsKey("groupLinks"))
                {
                    JArray formattingGroupLinks = formatting["groupLinks"] as JArray;
                    if (formattingGroupLinks == null || !formattingGroupLinks.Any())
                        formatting.Remove("groupLinks");
                }

                if (formatting.HasValues)
                    SerializationHelper.MovePropertyIfExists(ref body, ref metadata, "formatting");
                else
                    body.Remove("formatting");
            }
            if (metadata.HasValues)
                body.Add(new JProperty("metadata", metadata));

            // embeds
            if (body.ContainsKey("embeds"))
            {
                JArray embedsArray = body["embeds"] as JArray;
                if (embedsArray == null || embedsArray.Count == 0)
                    body.Remove("embeds");
            }

            // raw data
            if (msg.RawData?.Any() == true)
            {
                body["data"] = new JObject(new JProperty("_placeholder", true), new JProperty("num", 0));
                return new SerializedMessageData(payload, msg.RawData.ToArray());
            }
            return new SerializedMessageData(payload);
        }

        /// <summary>Determines which type of chat message it is.</summary>
        /// <param name="messageBody">Raw body object of the message payload.</param>
        /// <returns>Type of the chat message.</returns>
        public static Type GetMessageType(JToken messageBody)
        {
            string mimeType = messageBody["mimeType"].ToObject<string>();

            // special case: private request response
            if (string.Equals(mimeType, ChatMessageTypes.PrivateRequestResponse, StringComparison.OrdinalIgnoreCase))
                return null;
            // special case: group action
            if (string.Equals(mimeType, ChatMessageTypes.GroupAction, StringComparison.OrdinalIgnoreCase))
                return typeof(GroupActionChatEvent);
            // normal case: chat message
            return typeof(ChatMessage);
        }
    }
}
