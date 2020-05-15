using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace TehGM.Wolfringo.Messages.Serialization
{
    public class JsonMessageSerializer<T> : IMessageSerializer where T : IWolfMessage
    {
        public IWolfMessage Deserialize(string command, string payload, IEnumerable<byte[]> buffers)
            => JToken.Parse(payload).ToObject<T>();

        public string Serialize(IWolfMessage message)
            => JToken.FromObject(message).ToString(Formatting.None);
    }
}
