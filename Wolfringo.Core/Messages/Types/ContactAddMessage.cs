using Newtonsoft.Json;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>A message for adding a user as contact.</summary>
    public class ContactAddMessage : IWolfMessage
    {
        /// <inheritdoc/>
        /// <remarks>Equals to <see cref="MessageEventNames.SubscriberContactAdd"/>.</remarks>
        [JsonIgnore]
        public string EventName => MessageEventNames.SubscriberContactAdd;

        /// <summary>ID of user being added as contact.</summary>
        [JsonProperty("id")]
        public uint UserID { get; private set; }

        /// <summary>Creates a message instance.</summary>
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
