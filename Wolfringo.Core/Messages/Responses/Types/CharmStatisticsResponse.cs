using Newtonsoft.Json;

namespace TehGM.Wolfringo.Messages.Responses
{
    /// <summary>Wolf server's response for <see cref="CharmStatisticsMessage"/>.</summary>
    public class CharmStatisticsResponse : WolfResponse, IWolfResponse
    {
        /// <summary>Tag for the response.</summary>
        [JsonProperty("etag")]
        public string Etag { get; private set; }

        /// <summary>ID of the user.</summary>
        [JsonProperty("subscriberId")]
        public uint UserID { get; private set; }
        /// <summary>Total charms active.</summary>
        [JsonProperty("totalActive")]
        public int? TotalActive { get; private set; }
        /// <summary>Total charms expired.</summary>
        [JsonProperty("totalExpired")]
        public int? TotalExpired { get; private set; }
        /// <summary>Total charms received as a gift.</summary>
        [JsonProperty("totalGiftedReceived")]
        public int? TotalGiftsReceived { get; private set; }
        /// <summary>Total charms gifted to other users.</summary>
        [JsonProperty("totalGiftedSent")]
        public int? TotalGiftsSent { get; private set; }
        /// <summary>Total charms.</summary>
        [JsonProperty("totalLifetime")]
        public int? TotalLifetime { get; private set; }

        /// <summary>Creates a response instance.</summary>
        [JsonConstructor]
        protected CharmStatisticsResponse() : base() { }
    }
}
