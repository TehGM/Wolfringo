using Newtonsoft.Json;
using System.Collections.Generic;

namespace TehGM.Wolfringo.Messages
{
    public class SubscribeToPmMessage : IHeadersWolfMessage
    {
        [JsonIgnore]
        public string Command => MessageCommands.MessagePrivateSubscribe;
        [JsonIgnore]
        public IDictionary<string, object> Headers { get; } = new Dictionary<string, object>()
        {
            { "version", 2 }
        };
    }
}
