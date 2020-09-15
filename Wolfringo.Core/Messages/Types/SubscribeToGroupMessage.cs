using Newtonsoft.Json;
using System.Collections.Generic;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>A message for subscribing to group messages.</summary>
    [ResponseType(typeof(EntitiesSubscribeResponse))]
    public class SubscribeToGroupMessage : IHeadersWolfMessage
    {
        /// <inheritdoc/>
        [JsonIgnore]
        public string Command => MessageCommands.MessageGroupSubscribe;
        /// <inheritdoc/>
        [JsonIgnore]
        public IDictionary<string, object> Headers { get; } = new Dictionary<string, object>()
        {
            { "version", 3 }
        };

        /// <summary>ID of the group.</summary>
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public uint? GroupID { get; private set; }

        [JsonConstructor]
        public SubscribeToGroupMessage() { }

        /// <summary>Creates a message instance.</summary>
        /// <param name="groupID">ID of the group to subscribe to. Use null to subscribe to all.</param>
        public SubscribeToGroupMessage(uint? groupID) : this()
        {
            this.GroupID = groupID;
        }
    }
}
