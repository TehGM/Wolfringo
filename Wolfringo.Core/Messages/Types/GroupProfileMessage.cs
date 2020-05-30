using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo.Messages
{
    [ResponseType(typeof(GroupProfileResponse))]
    public class GroupProfileMessage : IWolfMessage, IHeadersWolfMessage
    {
        public static readonly IEnumerable<string> DefaultRequestEntities = new string[] { "base", "audioConfig", "audioCounts", "extended" };

        [JsonIgnore]
        public string Command => MessageCommands.GroupProfile;
        [JsonIgnore]
        public IDictionary<string, object> Headers { get; } = new Dictionary<string, object>()
        {
            { "version", 4 }
        };

        [JsonProperty("idList")]
        public IEnumerable<uint> RequestGroupIDs { get; private set; }
        [JsonProperty("subscribe")]
        public bool SubscribeToUpdates { get; private set; }
        [JsonProperty("entities")]
        public IEnumerable<string> RequestEntities { get; private set; }

        [JsonConstructor]
        private GroupProfileMessage() { }

        public GroupProfileMessage(IEnumerable<uint> groupIDs, IEnumerable<string> requestEntities, bool subscribe = true)
            : this()
        {
            if (groupIDs?.Any() != true)
                throw new ArgumentException("Must request at least one group ID", nameof(groupIDs));
            if (requestEntities?.Any() != true)
                throw new ArgumentException("Must request at least one entity type", nameof(requestEntities));

            this.RequestEntities = requestEntities;
            this.RequestGroupIDs = groupIDs;
            this.SubscribeToUpdates = subscribe;
        }

        public GroupProfileMessage(IEnumerable<uint> groupIDs, bool subscribe = true)
            : this(groupIDs, DefaultRequestEntities, subscribe) { }
    }
}
