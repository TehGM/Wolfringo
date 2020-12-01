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
        /// <remarks>Equals to <see cref="MessageEventNames.MessageGroupUnsubscribe"/>.</remarks>
        [JsonIgnore]
        public string EventName => MessageEventNames.MessageGroupUnsubscribe;

        /// <summary>IDs of the groups.</summary>
        [JsonProperty("idList")]
        public IEnumerable<uint> GroupIDs { get; private set; }

        /// <summary>Creates a message instance.</summary>
        [JsonConstructor]
        protected UnsubscribeFromGroupMessage() { }

        /// <summary>Creates a message instance.</summary>
        /// <param name="groupIDs">IDs of the groups to unsubscribe from.</param>
        public UnsubscribeFromGroupMessage(IEnumerable<uint> groupIDs) : this()
        {
            if (groupIDs != null)
                this.GroupIDs = new ReadOnlyCollection<uint>((groupIDs as IList<uint>) ?? groupIDs.ToArray());
        }
    }
}
