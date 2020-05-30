using Newtonsoft.Json;
using System.Collections.Generic;

namespace TehGM.Wolfringo.Messages
{
    public class ListCharmsMessage : IWolfMessage
    {
        [JsonIgnore]
        public string Command => MessageCommands.CharmList;

        [JsonProperty("idList", NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<uint> CharmIDs { get; private set; }

        [JsonConstructor]
        private ListCharmsMessage() { }

        public ListCharmsMessage(IEnumerable<uint> charmIDs)
        {
            this.CharmIDs = charmIDs;
        }
    }
}
