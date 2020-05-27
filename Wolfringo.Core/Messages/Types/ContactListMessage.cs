using Newtonsoft.Json;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo.Messages
{
    [ResponseType(typeof(ContactListResponse))]
    public class ContactListMessage : IWolfMessage
    {
        public string Command => MessageCommands.SubscriberContactList;

        [JsonProperty("subscribe")]
        public bool SubscribeToUpdates { get; private set; }

        public ContactListMessage(bool subscribe = true)
        {
            this.SubscribeToUpdates = subscribe;
        }
    }
}
