using Newtonsoft.Json;
using System.Collections.Generic;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo.Messages.Serialization
{
    public class JsonMessageSerializer<T> : IMessageSerializer where T : IWolfMessage
    {
        public IWolfMessage Deserialize(string command, string payload, IEnumerable<byte[]> buffers)
            => JsonConvert.DeserializeObject<T>(payload, SerializationHelper.SerializerSettings);

        public string Serialize(IWolfMessage message)
            => JsonConvert.SerializeObject(message, SerializationHelper.SerializerSettings);
    }
}
