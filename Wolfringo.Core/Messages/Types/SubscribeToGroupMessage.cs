using Newtonsoft.Json;
using System.Collections.Generic;

namespace TehGM.Wolfringo.Messages
{
    public class SubscribeToGroupMessage : IHeadersWolfMessage
    {
        [JsonIgnore]
        public string Command => MessageCommands.MessageGroupSubscribe;
        [JsonIgnore]
        public IDictionary<string, object> Headers { get; } = new Dictionary<string, object>()
        {
            { "version", 3 }
        };
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public uint? GroupID { get; private set; }

        [JsonConstructor]
        public SubscribeToGroupMessage() { }

        public SubscribeToGroupMessage(uint? groupID) : this()
        {
            this.GroupID = groupID;
        }
    }
}
