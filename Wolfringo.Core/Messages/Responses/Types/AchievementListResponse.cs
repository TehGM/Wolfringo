using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo.Messages.Responses
{
    /// <summary>Wolf server's response for <see cref="AchievementListMessage"/>.</summary>
    public class AchievementListResponse : WolfResponse, IWolfResponse
    {
        /// <summary>Achievements.</summary>
        [JsonProperty("body")]
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
        protected AchievementListResponse() : base() { }
    }
}
