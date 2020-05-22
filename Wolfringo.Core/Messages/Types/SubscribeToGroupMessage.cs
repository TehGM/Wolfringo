using Newtonsoft.Json;
using System.Collections.Generic;

namespace TehGM.Wolfringo.Messages
{
    public class SubscribeToGroupMessage : IHeadersWolfMessage
    {
        [JsonIgnore]
        public string Command => MessageCommands.SubscribeToGroup;
        [JsonIgnore]
        public IDictionary<string, object> Headers { get; } = new Dictionary<string, object>()
        {
            { "version", 3 }
        };
    }
}
