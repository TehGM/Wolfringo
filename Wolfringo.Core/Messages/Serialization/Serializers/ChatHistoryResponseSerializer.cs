using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using TehGM.Wolfringo.Messages.Responses;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo.Messages.Serialization
{
    public class ChatHistoryResponseSerializer : DefaultResponseSerializer, IResponseSerializer
    {
        private static readonly Type _chatHistoryResponseType = typeof(ChatHistoryResponse);

        public override IWolfResponse Deserialize(Type responseType, SerializedMessageData responseData)
        {
            if (!_chatHistoryResponseType.IsAssignableFrom(responseType))
                throw new ArgumentException($"{typeof(ChatHistoryResponseSerializer).Name} only works with responses of type {_chatHistoryResponseType.FullName}", nameof(responseType));

            // first deserialize the json message
            ChatHistoryResponse result = (ChatHistoryResponse)base.Deserialize(responseType, responseData);

            // then assign binary data to each of the messages
            JArray responseBody = GetResponseJson(responseData)["body"] as JArray;
            if (responseBody == null)
                throw new ArgumentException("Chat history response requires to have a body property that is a JSON array", nameof(responseData));
            foreach (JToken responseChatMessage in responseBody)
            {
                Guid msgId = responseChatMessage["id"].ToObject<Guid>();
                ChatMessage msg = result.Messages.First(m => m.ID == msgId);
                JToken numProp = responseChatMessage["data"]["num"];
                int binaryIndex = numProp.ToObject<int>(SerializationHelper.DefaultSerializer);
                msg.RawData = responseData.BinaryMessages.ElementAt(binaryIndex).Skip(1).ToArray();
            }

            return result;
        }
    }
}
