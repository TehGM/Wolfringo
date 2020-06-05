using Newtonsoft.Json;
using System;

namespace TehGM.Wolfringo
{
    /// <summary>Charm ownership metadata.</summary>
    public class WolfCharmSubscription
    {
        /// <summary>ID of the subscription.</summary>
        [JsonProperty("id")]
        public uint SubscriptionID { get; private set; }
        /// <summary>ID of the charm.</summary>
        [JsonProperty("charmId")]
        public uint CharmID { get; private set; }
        /// <summary>ID of the user owning the charm.</summary>
        [JsonProperty("subscriberId")]
        public uint UserID { get; private set; }
        /// <summary>ID of the user that purchased the charm.</summary>
        [JsonProperty("sourceSubscriberId")]
        public uint PurchasedByUserID { get; private set; }
        /// <summary>Charm ownership's expiration time.</summary>
        [JsonProperty("expireTime")]
        public DateTime? ExpirationTime { get; private set; }
    }
}
