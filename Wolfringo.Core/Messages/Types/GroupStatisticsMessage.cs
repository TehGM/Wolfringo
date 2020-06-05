using Newtonsoft.Json;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>A message for requesting user's group statistics.</summary>
    /// <remarks>Uses <see cref="GroupStatisticsResponse"/> as response type.</remarks>
    [ResponseType(typeof(GroupStatisticsResponse))]
    public class GroupStatisticsMessage : IWolfMessage
    {
        /// <inheritdoc/>
        [JsonIgnore]
        public string Command => MessageCommands.GroupStats;

        /// <summary>ID of the group.</summary>
        [JsonProperty("id")]
        public uint GroupID { get; private set; }

        [JsonConstructor]
        private GroupStatisticsMessage() { }

        /// <summary>Creates a message instance.</summary>
        /// <param name="groupID">ID of the group to get statistics of.</param>
        public GroupStatisticsMessage(uint groupID) : this()
        {
            this.GroupID = groupID;
        }
    }
}
