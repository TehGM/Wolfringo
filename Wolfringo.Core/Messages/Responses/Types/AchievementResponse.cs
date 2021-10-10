using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo.Messages.Responses
{
    /// <summary>Wolf server's response for <see cref="AchievementMessage"/>.</summary>
    public class AchievementResponse : WolfResponse, IWolfResponse
    {
        /// <summary>Achievements.</summary>
        [JsonProperty("body")]
        [JsonConverter(typeof(ExtractValuesOnlyConverter<WolfAchievement>))]
        public IEnumerable<WolfAchievement> Achievements { get; private set; }

        [JsonIgnore]
        private IEnumerable<WolfAchievement> _flattenedAchievements;

        /// <summary>Gets list of achievements, with all child achievements surfaced to the top of the collection.</summary>
        public IEnumerable<WolfAchievement> GetFlattenedAchievementList()
        {
            if (this.Achievements == null || !this.Achievements.Any())
                return this.Achievements;
            if (_flattenedAchievements == null)
                this._flattenedAchievements = NestedEntitiesHelper.FlattenAchievementsList(this.Achievements);
            return this._flattenedAchievements;
        }

        /// <summary>Creates a response instance.</summary>
        [JsonConstructor]
        protected AchievementResponse() : base() { }
    }
}
