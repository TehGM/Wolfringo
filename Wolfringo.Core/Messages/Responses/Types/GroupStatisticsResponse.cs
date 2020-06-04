using Newtonsoft.Json;

namespace TehGM.Wolfringo.Messages.Responses
{
    public class GroupStatisticsResponse : WolfResponse, IWolfResponse
    {
        [JsonProperty("body")]
        public WolfGroupStatistics GroupStatistics { get; private set; }
    }
}
