using Newtonsoft.Json;

namespace TehGM.Wolfringo.Messages
{
    public class ContactDeleteMessage : IWolfMessage
    {
        public string Command => MessageCommands.SubscriberContactDelete;

        [JsonProperty("id")]
        public uint UserID { get; private set; }

        [JsonConstructor]
        private ContactDeleteMessage() { }

        public ContactDeleteMessage(uint userID) : this()
        {
            this.UserID = userID;
        }
    }
}
