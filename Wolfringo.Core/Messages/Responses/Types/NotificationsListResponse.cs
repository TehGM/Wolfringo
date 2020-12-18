using Newtonsoft.Json;
using System.Collections.Generic;

namespace TehGM.Wolfringo.Messages.Responses
{
    /// <summary>Wolf server's response for <see cref="NotificationsListMessage"/>.</summary>
    public class NotificationsListResponse : WolfResponse, IWolfResponse
    {
        /// <summary>Notifications.</summary>
        [JsonProperty("body")]
        public IEnumerable<WolfNotification> Notifications { get; private set; }

        /// <summary>Creates a response instance.</summary>
        [JsonConstructor]
        protected NotificationsListResponse() : base() { }
    }
}
