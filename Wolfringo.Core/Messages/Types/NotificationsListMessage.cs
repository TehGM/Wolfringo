using Newtonsoft.Json;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>A message for requesting notifications.</summary>
    /// <remarks>Uses <see cref="NotificationsListResponse"/> as response type.</remarks>
    [ResponseType(typeof(NotificationsListResponse))]
    public class NotificationsListMessage : IWolfMessage
    {
        /// <inheritdoc/>
        [JsonIgnore]
        public string EventName => MessageEventNames.NotificationList;

        private const WolfLanguage _defaultLanguage = WolfLanguage.English;

        /// <summary>Language of notifications translations.</summary>
        [JsonProperty("language")]
        public WolfLanguage Language { get; private set; }
        /// <summary>Device type.</summary>
        [JsonProperty("deviceType")]
        public WolfDevice Device { get; private set; }

        /// <summary>Creates a message instance.</summary>
        /// <param name="language">Language to request notifications in.</param>
        /// <param name="device">Device type to send to the server.</param>
        public NotificationsListMessage(WolfLanguage language = _defaultLanguage, WolfDevice device = WolfDevice.Bot)
        {
            this.Language = language;
            this.Device = device;
        }

        /// <summary>Creates a message instance.</summary>
        /// <param name="device">Device type to send to the server.</param>
        public NotificationsListMessage(WolfDevice device)
            : this(_defaultLanguage, device: device) { }
    }
}
