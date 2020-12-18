using Newtonsoft.Json;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>A message for requesting user's achievements list.</summary>
    /// <remarks>Uses <see cref="UserAchievementListResponse"/> as response type.</remarks>
    [ResponseType(typeof(UserAchievementListResponse))]
    public class UserAchievementListMessage : IWolfMessage
    {
        /// <inheritdoc/>
        /// <remarks>Equals to <see cref="MessageEventNames.AchievementSubscriberList"/>.</remarks>
        [JsonIgnore]
        public string EventName => MessageEventNames.AchievementSubscriberList;

        /// <summary>ID of the user.</summary>
        [JsonProperty("id")]
        public uint UserID { get; private set; }

        /// <summary>Creates a message instance.</summary>
        [JsonConstructor]
        protected UserAchievementListMessage() { }

        /// <summary>Creates a message instance.</summary>
        /// <param name="userID">ID of the user to get achievements of.</param>
        public UserAchievementListMessage(uint userID) : this()
        {
            this.UserID = userID;
        }
    }
}
