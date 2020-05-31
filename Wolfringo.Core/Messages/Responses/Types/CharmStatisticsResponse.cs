using Newtonsoft.Json;

namespace TehGM.Wolfringo.Messages.Responses
{
    public class CharmStatisticsResponse : WolfResponse, IWolfResponse
    {
        [JsonProperty("etag")]
        public string Etag { get; private set; }

        [JsonProperty("subscriberId")]
        public uint UserID { get; private set; }
        [JsonProperty("totalActive")]
        public int? TotalActive { get; private set; }
        [JsonProperty("totalExpired")]
        public int? TotalExpired { get; private set; }
        [JsonProperty("totalGiftedReceived")]
        public int? TotalGiftsReceived { get; private set; }
        [JsonProperty("totalGiftedSent")]
        public int? TotalGiftsSent { get; private set; }
        [JsonProperty("totalLifetime")]
        public int? TotalLifetime { get; private set; }
    }
}
