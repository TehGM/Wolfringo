using System.Collections.Generic;
using Newtonsoft.Json;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>A message for requesting user's achievements list.</summary>
    /// <remarks>Uses <see cref="EntityAchievementListResponse"/> as response type.</remarks>
    [ResponseType(typeof(EntityAchievementListResponse))]
    public class UserAchievementListMessage : IWolfMessage, IHeadersWolfMessage
    {
        /// <inheritdoc/>
        /// <remarks>Equals to <see cref="MessageEventNames.AchievementSubscriberList"/>.</remarks>
        [JsonIgnore]
        public string EventName => MessageEventNames.AchievementSubscriberList;

        /// <inheritdoc/>
        [JsonIgnore]
        public IDictionary<string, object> Headers { get; } = new Dictionary<string, object>()
        {
            { "version", 2 }
        };

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
