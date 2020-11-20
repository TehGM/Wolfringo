using Newtonsoft.Json;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>A message for requesting user's expired charms subscriptions.</summary>
    /// <remarks>Uses <see cref="UserCharmsListResponse"/> as response type.</remarks>
    [ResponseType(typeof(UserCharmsListResponse))]
    public class UserExpiredCharmsListMessage : IWolfMessage
    {
        public string EventName => MessageEventNames.CharmSubscriberExpiredList;

        /// <summary>ID of the user.</summary>
        [JsonProperty("id")]
        public uint UserID { get; private set; }

        [JsonConstructor]
        protected UserExpiredCharmsListMessage() { }

        /// <summary>Creates a message instance.</summary>
        /// <param name="userID">ID of the user to get expired charms of.</param>
        public UserExpiredCharmsListMessage(uint userID) : this()
        {
            this.UserID = userID;
        }
    }
}
