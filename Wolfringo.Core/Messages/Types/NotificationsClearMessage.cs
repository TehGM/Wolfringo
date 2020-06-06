using Newtonsoft.Json;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>A message for clearing notifications.</summary>
    public class NotificationsClearMessage : IWolfMessage
    {
        /// <inheritdoc/>
        [JsonIgnore]
        public string Command => MessageCommands.NotificationListClear;
    }
}
