using Newtonsoft.Json;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo.Messages
{
    [ResponseType(typeof(AchievementListResponse))]
    public class AchievementListMessage : IWolfMessage
    {
        [JsonIgnore]
        public string Command => MessageCommands.AchievementList;

        [JsonProperty("language")]
        public WolfLanguage Language { get; private set; }

        [JsonConstructor]
        private AchievementListMessage() { }

        public AchievementListMessage(WolfLanguage language)
        {
            this.Language = language;
        }
    }
}
