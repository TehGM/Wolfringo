using Newtonsoft.Json;
using System;

namespace TehGM.Wolfringo
{
    public class WolfCharmSubscription
    {
        [JsonProperty("id")]
        public uint SubscriptionID { get; private set; }
        [JsonProperty("charmId")]
        public uint CharmID { get; private set; }
        [JsonProperty("subscriberId")]
        public uint UserID { get; private set; }
        [JsonProperty("sourceSubscriberId")]
        public uint PurchasedByUserID { get; private set; }
        [JsonProperty("expireTime")]
        public DateTime? ExpirationTime { get; private set; }
    }
}
