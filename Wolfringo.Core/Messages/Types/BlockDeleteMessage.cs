using Newtonsoft.Json;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>A message for unblocking a user.</summary>
    public class BlockDeleteMessage : IWolfMessage
    {
        /// <inheritdoc/>
        /// <remarks>Equals to <see cref="MessageEventNames.SubscriberBlockDelete"/>.</remarks>
        [JsonIgnore]
        public string EventName => MessageEventNames.SubscriberBlockDelete;

        /// <summary>ID of user being unblocked.</summary>
        [JsonProperty("id")]
        public uint UserID { get; private set; }

        /// <summary>Creates a message instance.</summary>
        [JsonConstructor]
        protected BlockDeleteMessage() { }

        /// <summary>Creates a message instance.</summary>
        /// <param name="userID">ID of user to unblock.</param>
        public BlockDeleteMessage(uint userID) : this()
        {
            this.UserID = userID;
        }
    }
}
