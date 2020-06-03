using Newtonsoft.Json;

namespace TehGM.Wolfringo.Messages
{
    public class ContactAddMessage : IWolfMessage
    {
        public string Command => MessageCommands.SubscriberContactAdd;

        [JsonProperty("id")]
        public uint UserID { get; private set; }

        [JsonConstructor]
        private ContactAddMessage() { }

        public ContactAddMessage(uint userID) : this()
        {
            this.UserID = userID;
        }
    }
}
