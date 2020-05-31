using Newtonsoft.Json;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo.Messages
{
    [ResponseType(typeof(CharmStatisticsResponse))]
    public class CharmStatisticsMessage : IWolfMessage
    {
        public string Command => MessageCommands.CharmSubscriberStatistics;

        [JsonProperty("id")]
        public uint UserID { get; private set; }

        [JsonConstructor]
        private CharmStatisticsMessage() { }

        public CharmStatisticsMessage(uint userID) : this()
        {
            this.UserID = userID;
        }
    }
}
