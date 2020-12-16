using Newtonsoft.Json.Linq;
using System;
using TehGM.Wolfringo.Messages.Responses;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo.Messages.Serialization
{
    /// <summary>Serializer for basic responses that don't have binary data.</summary>
    public class DefaultResponseSerializer : IResponseSerializer
    {
        private static readonly Type _baseResponseType = typeof(IWolfResponse);

        /// <inheritdoc/>
        public virtual IWolfResponse Deserialize(Type responseType, SerializedMessageData responseData)
        {
            ThrowIfInvalidType(responseType);

            JToken responseJson = GetResponseJson(responseData);
            object result = responseJson.ToObject(responseType, SerializationHelper.DefaultSerializer);
            // if response has body or headers, further use it to populate the response entity
            responseJson.FlattenCommonProperties(result);
            return (IWolfResponse)result;
        }

        /// <summary>Gets response json, stripping off wrapping array.</summary>
        /// <param name="responseData">Serialized response data.</param>
        /// <returns>Core response payload.</returns>
        protected static JToken GetResponseJson(SerializedMessageData responseData)
            => GetResponseJson(responseData.Payload);

        /// <summary>Gets response json, stripping off wrapping array.</summary>
        /// <param name="payload">JSON response payload.</param>
        /// <returns>Core response payload.</returns>
        protected static JToken GetResponseJson(JToken payload)
            => payload is JArray ? payload.First : payload;

        /// <summary>Throws if response type is not supported by this serializer.</summary>
        /// <param name="responseType">Type of the response.</param>
        protected virtual void ThrowIfInvalidType(Type responseType)
        {
            if (!_baseResponseType.IsAssignableFrom(responseType))
                throw new ArgumentException($"Response type must implement {_baseResponseType.FullName}", nameof(responseType));
        }
    }
}
