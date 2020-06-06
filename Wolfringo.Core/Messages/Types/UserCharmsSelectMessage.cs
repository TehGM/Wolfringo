using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>A message for setting active selected charm.</summary>
    public class UserCharmsSelectMessage : IWolfMessage
    {
        /// <inheritdoc/>
        [JsonIgnore]
        public string Command => MessageCommands.CharmSubscriberSetSelected;

        /// <summary>Selected charms IDs.</summary>
        [JsonProperty("selectedList")]
        [JsonConverter(typeof(ObjectPropertiesDictionaryConverter<int, uint>), "position", "charmId")]
        public IReadOnlyDictionary<int, uint> SelectedCharmsIDs { get; private set; }

        [JsonConstructor]
        protected UserCharmsSelectMessage() { }

        /// <summary>Creates a message instance.</summary>
        /// <param name="charmsPositionsAndIDs">Positions and IDs of charms to set as selected.</param>
        public UserCharmsSelectMessage(IDictionary<int, uint> charmsPositionsAndIDs)
        {
            this.SelectedCharmsIDs = new ReadOnlyDictionary<int, uint>(charmsPositionsAndIDs ?? new Dictionary<int, uint>());
        }
    }
}
