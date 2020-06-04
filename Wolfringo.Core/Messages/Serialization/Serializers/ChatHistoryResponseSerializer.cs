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
            // first deserialize the json message
            ChatHistoryResponse result = (ChatHistoryResponse)base.Deserialize(responseType, responseData);

            // then assign binary data to each of the messages
            JArray responseBody = GetResponseJson(responseData)["body"] as JArray;
            if (responseBody == null)
                throw new ArgumentException("Chat history response requires to have a body property that is a JSON array", nameof(responseData));
            foreach (JToken responseChatMessage in responseBody)
            {
                Guid msgId = responseChatMessage["id"].ToObject<Guid>();
                IChatMessage msg = result.Messages.First(m => m.ID == msgId);
                JToken numProp = responseChatMessage["data"]["num"];
                int binaryIndex = numProp.ToObject<int>(SerializationHelper.DefaultSerializer);
                ChatMessageSerializer.PopulateMessageData(ref msg, responseData.BinaryMessages.ElementAt(binaryIndex));
            }

            return result;
        }

        protected override void ThrowIfInvalidType(Type responseType)
        {
            base.ThrowIfInvalidType(responseType);
            if (!_chatHistoryResponseType.IsAssignableFrom(responseType))
                throw new ArgumentException($"{this.GetType().Name} only works with responses of type {_chatHistoryResponseType.FullName}", nameof(responseType));
        }
    }
}
