using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace TehGM.Wolfringo.Messages.Serialization
{
    public class SerializedMessageData
    {
        public JToken Payload { get; }
        public IEnumerable<byte[]> BinaryMessages { get; }

        public SerializedMessageData(JToken payload, IEnumerable<byte[]> binaryMessages)
        {
            this.Payload = payload;
            this.BinaryMessages = binaryMessages;
        }

        public SerializedMessageData(JToken payload, byte[] binaryData)
            : this(payload, new byte[][] { binaryData }) { }

        public SerializedMessageData(JToken payload)
            : this(payload, Enumerable.Empty<byte[]>()) { }
    }
}
