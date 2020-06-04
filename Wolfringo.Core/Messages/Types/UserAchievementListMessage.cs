using Newtonsoft.Json;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo.Messages
{
    [ResponseType(typeof(UserAchievementListResponse))]
    public class UserAchievementListMessage : IWolfMessage
    {
        public string Command => MessageCommands.AchievementSubscriberList;

        [JsonProperty("id")]
        public uint UserID { get; private set; }

        [JsonConstructor]
        private UserAchievementListMessage() { }

        public UserAchievementListMessage(uint userID) : this()
        {
            this.UserID = userID;
        }
    }
}
