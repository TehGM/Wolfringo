using Newtonsoft.Json.Linq;
using System;
using TehGM.Wolfringo.Messages.Responses;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo.Messages.Serialization
{
    public class DefaultResponseSerializer : IResponseSerializer
    {
        private static readonly Type _baseResponseType = typeof(IWolfResponse);

        public virtual IWolfResponse Deserialize(Type responseType, SerializedMessageData responseData)
        {
            if (!_baseResponseType.IsAssignableFrom(responseType))
                throw new ArgumentException($"Response type must implement {_baseResponseType.FullName}", nameof(responseType));

            JToken responseJson = (responseData.Payload is JArray) ? responseData.Payload.First : responseData.Payload;
            object result = responseJson.ToObject(responseType, SerializationHelper.DefaultSerializer);
            // if response has body or headers, further use it to populate the response entity
            responseJson.PopulateObject(ref result, "headers");
            responseJson.PopulateObject(ref result, "body");
            return (IWolfResponse)result;
        }
    }
}
