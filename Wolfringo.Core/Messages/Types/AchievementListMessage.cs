using Newtonsoft.Json;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>A message for requesting achievement list.</summary>
    /// <remarks>Uses <see cref="AchievementListResponse"/> as response type.</remarks>
    [ResponseType(typeof(AchievementListResponse))]
    public class AchievementListMessage : IWolfMessage
    {
        /// <inheritdoc/>
        [JsonIgnore]
        public string EventName => MessageEventNames.AchievementList;

        /// <summary>Language of achievement translations.</summary>
        [JsonProperty("language")]
        public WolfLanguage Language { get; private set; }

        [JsonConstructor]
        protected AchievementListMessage() { }

        /// <summary>Creates a message instance.</summary>
        /// <param name="language">Language of achievements.</param>
        public AchievementListMessage(WolfLanguage language)
        {
            this.Language = language;
        }
    }
}
