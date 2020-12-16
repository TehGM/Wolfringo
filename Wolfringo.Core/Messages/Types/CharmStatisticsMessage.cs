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
        /// <remarks>Equals to <see cref="MessageEventNames.CharmSubscriberStatistics"/>.</remarks>
        [JsonIgnore]
        public string EventName => MessageEventNames.CharmSubscriberStatistics;

        /// <summary>ID of user of requested charms statistics.</summary>
        [JsonProperty("id")]
        public uint UserID { get; private set; }

        /// <summary>Creates a message instance.</summary>
        [JsonConstructor]
        protected CharmStatisticsMessage() { }

        /// <summary>Creates a message instance.</summary>
        /// <param name="userID">ID of user to request charms statistics of.</param>
        public CharmStatisticsMessage(uint userID) : this()
        {
            this.UserID = userID;
        }
    }
}
