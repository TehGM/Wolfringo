using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo.Messages.Serialization
{
    public class ChatMessageSerializer : IMessageSerializer
    {
        public IWolfMessage Deserialize(string command, SerializedMessageData messageData)
        {
            // deserialize message
            Type msgType = GetMessageType(messageData.Payload["body"]);
            IChatMessage result = (IChatMessage)messageData.Payload.ToObject(msgType, SerializationHelper.DefaultSerializer);
            messageData.Payload.FlattenCommonProperties(result);

            // parse and populate binary data
            PopulateMessageData(ref result, messageData.BinaryMessages.First());

            return result;
        }

        public SerializedMessageData Serialize(IWolfMessage message)
        {
            JObject payload = message.SerializeJsonPayload();
            IChatMessage msg = (IChatMessage)message;
            if (msg.RawData?.Any() == true)
            {
                payload["body"]["data"] = new JObject(new JProperty("_placeholder", true), new JProperty("num", 0));
                return new SerializedMessageData(payload, msg.RawData.ToArray());
            }
            return new SerializedMessageData(payload);
        }

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

        public static void PopulateMessageData<T>(ref T message, IEnumerable<byte> data) where T : IChatMessage
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            if (data.Any() == true && data.First() == 4)
                data = data.Skip(1);


            if (message.RawData == null || !(message.RawData is ICollection<byte> byteCollection) || byteCollection.IsReadOnly)
                throw new InvalidOperationException($"Cannot populate raw data in {message.GetType().Name} as the collection is read only");
            byteCollection.Clear();
            // if it's a list, we can do it in a more performant way
            if (message.RawData is List<byte> byteList)
                byteList.AddRange(data);
            // otherwise do it one by one
            else
            {
                foreach (byte b in data)
                    byteCollection.Add(b);
            }
        }
    }
}
