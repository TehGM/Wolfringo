using Newtonsoft.Json;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>A message for blocking a user.</summary>
    public class BlockAddMessage : IWolfMessage
    {
        /// <inheritdoc/>
        [JsonIgnore]
        public string Command => MessageCommands.SubscriberBlockAdd;

        /// <summary>ID of user being blocked.</summary>
        [JsonProperty("id")]
        public uint UserID { get; private set; }

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
