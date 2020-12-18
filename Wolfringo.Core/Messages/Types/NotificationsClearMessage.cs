using Newtonsoft.Json;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>A message for clearing notifications.</summary>
    public class NotificationsClearMessage : IWolfMessage
    {
        /// <inheritdoc/>
        /// <remarks>Equals to <see cref="MessageEventNames.NotificationListClear"/>.</remarks>
        [JsonIgnore]
        public string EventName => MessageEventNames.NotificationListClear;
    }
}
