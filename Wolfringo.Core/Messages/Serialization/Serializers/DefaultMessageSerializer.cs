using Newtonsoft.Json.Linq;
using System.Linq;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo.Messages.Serialization
{
    /// <summary>Serializer for basic messages that don't have binary data.</summary>
    /// <typeparam name="T"></typeparam>
    public class DefaultMessageSerializer<T> : IMessageSerializer where T : IWolfMessage
    {
        public virtual IWolfMessage Deserialize(string command, SerializedMessageData messageData)
        {
            IWolfMessage result = messageData.Payload.ToObject<T>(SerializationHelper.DefaultSerializer);
            messageData.Payload.FlattenCommonProperties(ref result);
            return result;
        }

        public virtual SerializedMessageData Serialize(IWolfMessage message)
            => new SerializedMessageData(GetJsonPayload(message));

        public static JObject GetJsonPayload(IWolfMessage message)
        {
            JObject payload = new JObject();
            JToken body = JToken.FromObject(message, SerializationHelper.DefaultSerializer);
            if (body.HasValues)
                payload.Add(new JProperty("body", body));
            if (message is IHeadersWolfMessage headersMessage && headersMessage.Headers?.Any() == true)
                payload.Add(new JProperty("headers", JToken.FromObject(headersMessage.Headers, SerializationHelper.DefaultSerializer)));
            return payload;
        }
    }
}
