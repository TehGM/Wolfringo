using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using TehGM.Wolfringo.Messages.Responses;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo.Messages.Serialization
{
    /// <summary>Serializer for chat history responses.</summary>
    /// <remarks>This special serializer parses binary messages for all nested chat messages in chat history array.</remarks>
    public class ChatHistoryResponseSerializer : DefaultResponseSerializer, IResponseSerializer
    {
        private static readonly Type _chatHistoryResponseType = typeof(ChatHistoryResponse);

        /// <inheritdoc/>
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
                JToken numProp = responseChatMessage["data"]?["num"];
                if (numProp != null)
                {
                    int binaryIndex = numProp.ToObject<int>(SerializationHelper.DefaultSerializer);
                    SerializationHelper.PopulateMessageRawData(ref msg, responseData.BinaryMessages.ElementAt(binaryIndex));
                }
            }

            return result;
        }

        /// <inheritdoc/>
        protected override void ThrowIfInvalidType(Type responseType)
        {
            base.ThrowIfInvalidType(responseType);
            if (!_chatHistoryResponseType.IsAssignableFrom(responseType))
                throw new ArgumentException($"{this.GetType().Name} only works with responses of type {_chatHistoryResponseType.FullName}", nameof(responseType));
        }
    }
}
