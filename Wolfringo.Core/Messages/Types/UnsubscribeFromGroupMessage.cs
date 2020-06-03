using Newtonsoft.Json;
using System.Collections.Generic;

namespace TehGM.Wolfringo.Messages
{
    public class UnsubscribeFromGroupMessage : IWolfMessage
    {
        public string Command => MessageCommands.MessageGroupUnsubscribe;

        [JsonProperty("idList")]
        public IEnumerable<uint> GroupIDs { get; private set; }

        [JsonConstructor]
        private UnsubscribeFromGroupMessage() { }

        public UnsubscribeFromGroupMessage(IEnumerable<uint> groupIDs) : this()
        {
            this.GroupIDs = groupIDs;
        }
    }
}
