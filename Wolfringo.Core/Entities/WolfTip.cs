using Newtonsoft.Json;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo
{
    /// <summary>Represents a wolf message tip.</summary>
    public class WolfTip
    {
        /// <summary>ID of the charm used for tip display.</summary>
        [JsonProperty("id")]
        public uint CharmID { get; private set; }
        /// <summary>Cost of the tip.</summary>
        [JsonProperty("credits", NullValueHandling = NullValueHandling.Ignore)]
        public int? CreditsPrice { get; private set; }
        /// <summary>Number of tips of this type.</summary>
        [JsonProperty("quantity")]
        public int Quantity { get; private set; }


        /// <summary>ID of the user that tipped the message.</summary>
        [JsonProperty("subscriber", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ValueOrPropertyConverter), "id")]
        public uint? SenderID { get; private set; }
        [JsonProperty("magnitude", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public int Magnitude { get; private set; }

        [JsonConstructor]
        protected WolfTip() { }

        /// <summary>Creates a new tip instance, which then can be sent for tipping a message.</summary>
        /// <param name="charmID">ID of the charm to use as the tip.</param>
        /// <param name="quantity">Count of the tips to give at once.</param>
        public WolfTip(uint charmID, int quantity) : this()
        {
            this.CharmID = charmID;
            this.Quantity = quantity;
        }

        /// <summary>Tip's context type.</summary>
        public enum ContextType
        {
            Message,
            Stage
        }
    }
}
