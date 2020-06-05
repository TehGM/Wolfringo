using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>A message for unsubscribing from group messages.</summary>
    public class UnsubscribeFromGroupMessage : IWolfMessage
    {
        /// <inheritdoc/>
        [JsonIgnore]
        public string Command => MessageCommands.MessageGroupUnsubscribe;

        /// <summary>IDs of the groups.</summary>
        [JsonProperty("idList")]
        public IEnumerable<uint> GroupIDs { get; private set; }

        [JsonConstructor]
        private UnsubscribeFromGroupMessage() { }

        /// <summary>Creates a message instance.</summary>
        /// <param name="groupID">ID of the group to unsubscribe from.</param>
        public UnsubscribeFromGroupMessage(IEnumerable<uint> groupIDs) : this()
        {
            if (groupIDs != null)
                this.GroupIDs = new ReadOnlyCollection<uint>((groupIDs as IList<uint>) ?? groupIDs.ToArray());
        }
    }
}
