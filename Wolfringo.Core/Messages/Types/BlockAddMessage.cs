using Newtonsoft.Json;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>A message for blocking a user.</summary>
    public class BlockAddMessage : IWolfMessage
    {
        /// <inheritdoc/>
        /// <remarks>Equals to <see cref="MessageEventNames.SubscriberBlockAdd"/>.</remarks>
        [JsonIgnore]
        public string EventName => MessageEventNames.SubscriberBlockAdd;

        /// <summary>ID of user being blocked.</summary>
        [JsonProperty("id")]
        public uint UserID { get; private set; }

        /// <summary>Creates a message instance.</summary>
        [JsonConstructor]
        protected BlockAddMessage() { }

        /// <summary>Creates a message instance.</summary>
        /// <param name="userID">ID of user to block.</param>
        public BlockAddMessage(uint userID) : this()
        {
            this.UserID = userID;
        }
    }
}
