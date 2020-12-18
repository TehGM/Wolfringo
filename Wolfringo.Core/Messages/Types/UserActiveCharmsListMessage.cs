using Newtonsoft.Json;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>A message for requesting user's active charms subscriptions.</summary>
    /// <remarks>Uses <see cref="UserCharmsListResponse"/> as response type.</remarks>
    [ResponseType(typeof(UserCharmsListResponse))]
    public class UserActiveCharmsListMessage : IWolfMessage
    {
        /// <inheritdoc/>
        /// <remarks>Equals to <see cref="MessageEventNames.CharmSubscriberActiveList"/>.</remarks>
        [JsonIgnore]
        public string EventName => MessageEventNames.CharmSubscriberActiveList;

        /// <summary>ID of the user.</summary>
        [JsonProperty("id")]
        public uint UserID { get; private set; }

        /// <summary>Creates a message instance.</summary>
        [JsonConstructor]
        protected UserActiveCharmsListMessage() { }

        /// <summary>Creates a message instance.</summary>
        /// <param name="userID">ID of the user to get active charms of.</param>
        public UserActiveCharmsListMessage(uint userID) : this()
        {
            this.UserID = userID;
        }
    }
}
