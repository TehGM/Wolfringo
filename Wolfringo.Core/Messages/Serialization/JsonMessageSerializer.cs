using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo.Messages.Serialization
{
    /// <summary>Serializer for basic messages that don't have binary data.</summary>
    /// <typeparam name="T"></typeparam>
    public class JsonMessageSerializer<T> : IMessageSerializer where T : IWolfMessage
    {
        public IWolfMessage Deserialize(string command, JToken payload, IEnumerable<byte[]> binaryMessages)
            => payload.ToObject<T>(SerializationHelper.DefaultSerializer);

        public JToken Serialize(IWolfMessage message)
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
