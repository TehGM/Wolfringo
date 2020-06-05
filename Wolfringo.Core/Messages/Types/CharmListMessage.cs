using Newtonsoft.Json;
using System.Collections.Generic;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>A message for requesting charms list.</summary>
    /// <remarks>Uses <see cref="CharmListResponse"/> as response type.</remarks>
    [ResponseType(typeof(CharmListResponse))]
    public class CharmListMessage : IWolfMessage
    {
        /// <inheritdoc/>
        [JsonIgnore]
        public string Command => MessageCommands.CharmList;

        /// <summary>List of requested charms IDs.</summary>
        [JsonProperty("idList", NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<uint> CharmIDs { get; private set; }

        [JsonConstructor]
        private CharmListMessage() { }

        /// <summary>Creates a message instance.</summary>
        /// <param name="charmIDs">List of charms IDs to request. Use null to request all.</param>
        public CharmListMessage(IEnumerable<uint> charmIDs)
        {
            this.CharmIDs = charmIDs;
        }
    }
}
