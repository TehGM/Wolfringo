using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>A message for requesting group profile.</summary>
    /// <remarks>Uses <see cref="GroupProfileResponse"/> as response type.</remarks>
    [ResponseType(typeof(GroupProfileResponse))]
    public class GroupProfileMessage : IWolfMessage, IHeadersWolfMessage
    {
        /// <summary>Default group entities to request.</summary>
        [JsonIgnore]
        public static readonly IEnumerable<string> DefaultRequestEntities = new string[] { "base", "audioConfig", "audioCounts", "extended" };

        /// <inheritdoc/>
        /// <remarks>Equals to <see cref="MessageEventNames.GroupProfile"/>.</remarks>
        [JsonIgnore]
        public string EventName => MessageEventNames.GroupProfile;
        /// <inheritdoc/>
        [JsonIgnore]
        public IDictionary<string, object> Headers { get; } = new Dictionary<string, object>()
        {
            { "version", 4 }
        };

        /// <summary>IDs of requested groups. Mutually exclusive with <see cref="RequestGroupName"/></summary>
        [JsonProperty("idList", NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<uint> RequestGroupIDs { get; private set; }
        /// <summary>Name of the requested group. Mutually exclusive with <see cref="RequestGroupIDs"/>.</summary>
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string RequestGroupName { get; private set; }

        /// <summary>Subscribe to groups' profile updates?</summary>
        [JsonProperty("subscribe")]
        public bool SubscribeToUpdates { get; private set; }
        /// <summary>Requested entities.</summary>
        [JsonProperty("entities")]
        public IEnumerable<string> RequestEntities { get; private set; }

        /// <summary>Creates a message instance.</summary>
        [JsonConstructor]
        protected GroupProfileMessage() { }

        /// <summary>Creates a message instance.</summary>
        /// <param name="groupIDs">IDs of the groups to request.</param>
        /// <param name="requestEntities">Names of entities to request.</param>
        /// <param name="subscribe">Subscribe to groups' profile updates?</param>
        public GroupProfileMessage(IEnumerable<uint> groupIDs, IEnumerable<string> requestEntities, bool subscribe = true)
            : this()
        {
            if (groupIDs?.Any() != true)
                throw new ArgumentException("Must request at least one group ID", nameof(groupIDs));
            if (requestEntities?.Any() != true)
                throw new ArgumentException("Must request at least one entity type", nameof(requestEntities));

            this.RequestEntities = new ReadOnlyCollection<string>((requestEntities as IList<string>) ?? requestEntities.ToArray());
            this.RequestGroupIDs = new ReadOnlyCollection<uint>((groupIDs as IList<uint>) ?? groupIDs.ToArray());
            this.SubscribeToUpdates = subscribe;
            this.RequestGroupName = null;
        }

        /// <summary>Creates a message instance.</summary>
        /// <param name="groupIDs">IDs of the groups to request.</param>
        /// <param name="subscribe">Subscribe to groups' profile updates?</param>
        public GroupProfileMessage(IEnumerable<uint> groupIDs, bool subscribe = true)
            : this(groupIDs, DefaultRequestEntities, subscribe) { }

        /// <summary>Creates a message instance.</summary>
        /// <param name="groupName">Name of the group to request.</param>
        /// <param name="requestEntities">Names of entities to request.</param>
        /// <param name="subscribe">Subscribe to groups' profile updates?</param>
        public GroupProfileMessage(string groupName, IEnumerable<string> requestEntities, bool subscribe = true) : this()
        {
            if (string.IsNullOrWhiteSpace(groupName))
                throw new ArgumentNullException(nameof(groupName));
            if (requestEntities?.Any() != true)
                throw new ArgumentException("Must request at least one entity type", nameof(requestEntities));

            this.RequestEntities = new ReadOnlyCollection<string>((requestEntities as IList<string>) ?? requestEntities.ToArray());
            this.RequestGroupIDs = null;
            this.SubscribeToUpdates = subscribe;
            this.RequestGroupName = groupName;
        }

        /// <summary>Creates a message instance.</summary>
        /// <param name="groupName">Name of the group to request.</param>
        /// <param name="subscribe">Subscribe to groups' profile updates?</param>
        public GroupProfileMessage(string groupName, bool subscribe = true)
            : this(groupName, DefaultRequestEntities, subscribe) { }
    }
}
