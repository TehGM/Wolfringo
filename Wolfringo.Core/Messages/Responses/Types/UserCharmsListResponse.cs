using Newtonsoft.Json;
using System.Collections.Generic;

namespace TehGM.Wolfringo.Messages.Responses
{
    /// <summary>Wolf server's response for <see cref="UserActiveCharmsListMessage"/> and <see cref="UserExpiredCharmsListMessage"/>.</summary>
    public class UserCharmsListResponse : WolfResponse, IWolfResponse
    {
        /// <summary>User's charms active or expired subscriptions.</summary>
        [JsonProperty("body")]
        public IEnumerable<WolfCharmSubscription> Charms { get; private set; }

        /// <summary>Creates a response instance.</summary>
        [JsonConstructor]
        protected UserCharmsListResponse() : base() { }
    }
}
