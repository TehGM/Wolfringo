using Newtonsoft.Json;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>A message for unsubscribing from private messages.</summary>
    public class UnsubscribeFromPrivateMessage : IWolfMessage
    {
        /// <inheritdoc/>
        [JsonIgnore]
        public string EventName => MessageEventNames.MessagePrivateUnsubscribe;
    }
}
