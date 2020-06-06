using Newtonsoft.Json;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>A message for requesting user's charms statistics.</summary>
    /// <remarks>Uses <see cref="CharmStatisticsResponse"/> as response type.</remarks>
    [ResponseType(typeof(CharmStatisticsResponse))]
    public class CharmStatisticsMessage : IWolfMessage
    {
        /// <inheritdoc/>
        [JsonIgnore]
        public string Command => MessageCommands.CharmSubscriberStatistics;

        /// <summary>ID of user of requested charms statistics.</summary>
        [JsonProperty("id")]
        public uint UserID { get; private set; }

        [JsonConstructor]
        protected CharmStatisticsMessage() { }

        /// <summary>Creates a message instance.</summary>
        /// <param name="userId">ID of user to request charms statistics of.</param>
        public CharmStatisticsMessage(uint userID) : this()
        {
            this.UserID = userID;
        }
    }
}
