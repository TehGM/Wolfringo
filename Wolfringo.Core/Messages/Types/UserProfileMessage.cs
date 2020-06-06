using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>A message for requesting user profile.</summary>
    /// <remarks>Uses <see cref="UserProfileResponse"/> as response type.</remarks>
    [ResponseType(typeof(UserProfileResponse))]
    public class UserProfileMessage : IWolfMessage, IHeadersWolfMessage
    {
        /// <inheritdoc/>
        [JsonIgnore]
        public string Command => MessageCommands.SubscriberProfile;
        /// <inheritdoc/>
        [JsonIgnore]
        public IDictionary<string, object> Headers { get; } = new Dictionary<string, object>()
        {
            { "version", 4 }
        };

        /// <summary>Requesting extended user details?</summary>
        [JsonProperty("extended")]
        public bool RequestExtendedDetails { get; private set; }
        /// <summary>IDs of requested users.</summary>
        [JsonProperty("idList")]
        public IEnumerable<uint> RequestUserIDs { get; private set; }
        /// <summary>Subscribe to users' profile updates?</summary>
        [JsonProperty("subscribe")]
        public bool SubscribeToUpdates { get; private set; }

        [JsonConstructor]
        protected UserProfileMessage() { }

        /// <summary>Creates a message instance.</summary>
        /// <param name="userIDs">IDs of users to request.</param>
        /// <param name="requestExtended">Requesting extended user details?</param>
        /// <param name="subscribe">Subscribe to users' profile updates?</param>
        public UserProfileMessage(IEnumerable<uint> userIDs, bool requestExtended = true, bool subscribe = true)
            : this()
        {
            if (userIDs?.Any() != true)
                throw new ArgumentException("Must request at least one user ID", nameof(userIDs));
            this.RequestUserIDs = new ReadOnlyCollection<uint>((userIDs as IList<uint>) ?? userIDs.ToArray());
            this.RequestExtendedDetails = requestExtended;
            this.SubscribeToUpdates = subscribe;
        }
    }
}
