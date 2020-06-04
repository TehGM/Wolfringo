using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo.Messages
{
    public class UserCharmsSelectMessage : IWolfMessage
    {
        public string Command => MessageCommands.CharmSubscriberSetSelected;

        [JsonProperty("selectedList")]
        [JsonConverter(typeof(ObjectPropertiesDictionaryConverter<int, uint>), "position", "charmId")]
        public IReadOnlyDictionary<int, uint> SelectedCharmsIDs { get; private set; }

        [JsonConstructor]
        private UserCharmsSelectMessage() { }

        public UserCharmsSelectMessage(IDictionary<int, uint> charmsPositionsAndIDs)
        {
            this.SelectedCharmsIDs = new ReadOnlyDictionary<int, uint>(charmsPositionsAndIDs ?? new Dictionary<int, uint>());
        }
    }
}
