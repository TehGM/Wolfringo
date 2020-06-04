using Newtonsoft.Json;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo.Messages
{
    [ResponseType(typeof(GroupStatisticsResponse))]
    public class GroupStatisticsMessage : IWolfMessage
    {
        public string Command => MessageCommands.GroupStats;

        [JsonProperty("id")]
        public uint GroupID { get; private set; }

        [JsonConstructor]
        private GroupStatisticsMessage() { }

        public GroupStatisticsMessage(uint groupID) : this()
        {
            this.GroupID = groupID;
        }
    }
}
