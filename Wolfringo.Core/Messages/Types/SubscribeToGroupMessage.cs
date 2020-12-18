using Newtonsoft.Json;
using System.Collections.Generic;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>A message for subscribing to group messages.</summary>
    /// <remarks>Uses <see cref="EntitiesSubscribeResponse"/> as response type.</remarks>
    [ResponseType(typeof(EntitiesSubscribeResponse))]
    public class SubscribeToGroupMessage : IHeadersWolfMessage
    {
        /// <inheritdoc/>
        /// <remarks>Equals to <see cref="MessageEventNames.MessageGroupSubscribe"/>.</remarks>
        [JsonIgnore]
        public string EventName => MessageEventNames.MessageGroupSubscribe;
        /// <inheritdoc/>
        [JsonIgnore]
        public IDictionary<string, object> Headers { get; } = new Dictionary<string, object>()
        {
            { "version", 3 }
        };

        /// <summary>ID of the group.</summary>
        /// <remarks>When this value is null, request will subscribe to messages from all groups.</remarks>
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public uint? GroupID { get; private set; }

        /// <summary>Creates a message instance.</summary>
        [JsonConstructor]
        public SubscribeToGroupMessage() { }

        /// <summary>Creates a message instance.</summary>
        /// <param name="groupID">ID of the group to subscribe to. Use null to subscribe to all.</param>
        /// <remarks>When <paramref name="groupID"/> is null, the request will subscribe to messages from all groups.</remarks>
        public SubscribeToGroupMessage(uint? groupID) : this()
        {
            this.GroupID = groupID;
        }
    }
}
