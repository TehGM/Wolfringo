using Newtonsoft.Json;
using System.Collections.Generic;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo.Messages
{
    [ResponseType(typeof(CharmListResponse))]
    public class CharmListMessage : IWolfMessage
    {
        [JsonIgnore]
        public string Command => MessageCommands.CharmList;

        [JsonProperty("idList", NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<uint> CharmIDs { get; private set; }

        [JsonConstructor]
        private CharmListMessage() { }

        public CharmListMessage(IEnumerable<uint> charmIDs)
        {
            this.CharmIDs = charmIDs;
        }
    }
}
