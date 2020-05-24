using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo.Messages
{
    [ResponseType(typeof(SubscriberProfileResponse))]
    public class SubscriberProfileMessage : IWolfMessage, IHeadersWolfMessage
    {
        [JsonIgnore]
        public string Command => MessageCommands.SubscriberProfile;
        [JsonIgnore]
        public IDictionary<string, object> Headers { get; } = new Dictionary<string, object>()
        {
            { "version", 4 }
        };

        [JsonProperty("extended")]
        public bool RequestExtendedDetails { get; private set; }
        [JsonProperty("idList")]
        public IEnumerable<uint> RequestUserIDs { get; private set; }
        [JsonProperty("subscribe")]
        public bool SubscribeToUpdates { get; private set; }

        [JsonConstructor]
        private SubscriberProfileMessage() { }

        public SubscriberProfileMessage(IEnumerable<uint> userIDs, bool requestExtended = false, bool subscribe = true)
            : this()
        {
            this.RequestUserIDs = userIDs;
            this.RequestExtendedDetails = requestExtended;
            this.SubscribeToUpdates = subscribe;
        }

        public SubscriberProfileMessage(uint userID, bool requestExtended = false, bool subscribe = true)
            : this(new uint[] { userID }, requestExtended, subscribe) { }
    }
}
