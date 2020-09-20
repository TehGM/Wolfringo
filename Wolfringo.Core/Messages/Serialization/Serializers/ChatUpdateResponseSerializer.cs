using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using TehGM.Wolfringo.Messages.Responses;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo.Messages.Serialization
{
    public class ChatUpdateResponseSerializer : DefaultResponseSerializer, IResponseSerializer
    {
        private static readonly Type _chatUpdateResponseType = typeof(ChatUpdateResponse);

        public override IWolfResponse Deserialize(Type responseType, SerializedMessageData responseData)
        {
            // first deserialize the json message
            ChatUpdateResponse result = (ChatUpdateResponse)base.Deserialize(responseType, responseData);

            // assign binary data if any
            if (responseData.BinaryMessages?.Any() == true)
            {
                JObject responseBody = GetResponseJson(responseData)["body"] as JObject;
                if (responseBody == null)
                    throw new ArgumentException("Chat update response requires to have a body property that is a JSON object", nameof(responseData));
                if (responseBody["data"] != null)
                    SerializationHelper.PopulateMessageRawData(ref result, responseData.BinaryMessages.First());
            }

            return result;
        }

        protected override void ThrowIfInvalidType(Type responseType)
        {
            base.ThrowIfInvalidType(responseType);
            if (!_chatUpdateResponseType.IsAssignableFrom(responseType))
                throw new ArgumentException($"{this.GetType().Name} only works with responses of type {_chatUpdateResponseType.FullName}", nameof(responseType));
        }
    }
}
