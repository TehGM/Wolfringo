using Newtonsoft.Json;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>A message for requesting charms list.</summary>
    /// <remarks>Uses <see cref="ContactListResponse"/> as response type.</remarks>
    [ResponseType(typeof(ContactListResponse))]
    public class ContactListMessage : IWolfMessage
    {
        /// <inheritdoc/>
        [JsonIgnore]
        public string Command => MessageCommands.SubscriberContactList;

        /// <summary>Subscribe to contacts' profile updates?</summary>
        [JsonProperty("subscribe")]
        public bool SubscribeToUpdates { get; private set; }

        /// <summary>Creates a message instance.</summary>
        /// <param name="subscribe">Subscribe to contacts' profile updates?</param>
        public ContactListMessage(bool subscribe = true)
        {
            this.SubscribeToUpdates = subscribe;
        }
    }
}
