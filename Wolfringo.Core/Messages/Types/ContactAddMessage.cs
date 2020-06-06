using Newtonsoft.Json;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>A message for adding a user as contact.</summary>
    public class ContactAddMessage : IWolfMessage
    {
        /// <inheritdoc/>
        [JsonIgnore]
        public string Command => MessageCommands.SubscriberContactAdd;

        /// <summary>ID of user being added as contact.</summary>
        [JsonProperty("id")]
        public uint UserID { get; private set; }

        [JsonConstructor]
        protected ContactAddMessage() { }

        /// <summary>Creates a message instance.</summary>
        /// <param name="userID">ID of the user to add.</param>
        public ContactAddMessage(uint userID) : this()
        {
            this.UserID = userID;
        }
    }
}
