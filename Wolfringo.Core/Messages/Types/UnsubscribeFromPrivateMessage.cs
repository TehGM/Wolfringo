using Newtonsoft.Json;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>A message for unsubscribing from private messages.</summary>
    public class UnsubscribeFromPrivateMessage : IWolfMessage
    {
        /// <inheritdoc/>
        /// <remarks>Equals to <see cref="MessageEventNames.MessagePrivateUnsubscribe"/>.</remarks>
        [JsonIgnore]
        public string EventName => MessageEventNames.MessagePrivateUnsubscribe;
    }
}
