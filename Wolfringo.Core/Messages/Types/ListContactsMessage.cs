using Newtonsoft.Json;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo.Messages
{
    [ResponseType(typeof(ListContactsResponse))]
    public class ListContactsMessage : IWolfMessage
    {
        public string Command => MessageCommands.SubscriberContactList;

        [JsonProperty("subscribe")]
        public bool SubscribeToUpdates { get; private set; }

        public ListContactsMessage(bool subscribe = true)
        {
            this.SubscribeToUpdates = subscribe;
        }
    }
}
