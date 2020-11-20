using Newtonsoft.Json;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>A message for removing a contact.</summary>
    public class ContactDeleteMessage : IWolfMessage
    {
        /// <inheritdoc/>
        [JsonIgnore]
        public string EventName => MessageEventNames.SubscriberContactDelete;

        /// <summary>ID of user being removed from contacts.</summary>
        [JsonProperty("id")]
        public uint UserID { get; private set; }

        [JsonConstructor]
        protected ContactDeleteMessage() { }

        /// <summary>Creates a message instance.</summary>
        /// <param name="userID">ID of the user to remove.</param>
        public ContactDeleteMessage(uint userID) : this()
        {
            this.UserID = userID;
        }
    }
}
