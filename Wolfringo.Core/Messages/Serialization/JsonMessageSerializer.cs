using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo.Messages.Serialization
{
    public class JsonMessageSerializer<T> : IMessageSerializer where T : IWolfMessage
    {
        public IWolfMessage Deserialize(string command, string payload, IEnumerable<byte[]> buffers)
            => JsonConvert.DeserializeObject<T>(payload, SerializationHelper.SerializerSettings);

        public JToken Serialize(IWolfMessage message)
        {
            JToken body = JToken.FromObject(message, SerializationHelper.DefaultSerializer);
            if (message is IHeadersWolfMessage headersMessage)
            {
                JObject payload = new JObject();
                if (body.HasValues)
                    payload.Add(new JProperty("body", body));
                if (headersMessage.Headers?.Any() == true)
                    payload.Add(new JProperty("headers", JToken.FromObject(headersMessage.Headers, SerializationHelper.DefaultSerializer)));
                return payload;
            }
            return body;
        } 
    }
}
