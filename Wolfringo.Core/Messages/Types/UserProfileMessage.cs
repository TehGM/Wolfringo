using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo.Messages
{
    [ResponseType(typeof(UserProfileResponse))]
    public class UserProfileMessage : IWolfMessage, IHeadersWolfMessage
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
        private UserProfileMessage() { }

        public UserProfileMessage(IEnumerable<uint> userIDs, bool requestExtended = true, bool subscribe = true)
            : this()
        {
            if (userIDs?.Any() != true)
                throw new ArgumentException("Must request at least one user ID", nameof(userIDs));
            this.RequestUserIDs = userIDs;
            this.RequestExtendedDetails = requestExtended;
            this.SubscribeToUpdates = subscribe;
        }
    }
}
