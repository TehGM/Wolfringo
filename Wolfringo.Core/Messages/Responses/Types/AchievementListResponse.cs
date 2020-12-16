using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

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
            {
                List<WolfAchievement> results = new List<WolfAchievement>(this.Achievements.Count());
                AddWithChildren(ref results, this.Achievements);
                _flattenedAchievements = results.AsEnumerable();
            }
            return _flattenedAchievements;

            void AddWithChildren(ref List<WolfAchievement> resultsList, IEnumerable<WolfAchievement> achievements)
            {
                if (achievements?.Any() != true)
                    return;

                // add all provided first
                resultsList.AddRange(achievements);

                // recursively add children of each
                foreach (WolfAchievement achiv in achievements)
                    AddWithChildren(ref resultsList, achiv.ChildAchievements);
            }
        }

        /// <summary>Creates a response instance.</summary>
        [JsonConstructor]
        protected AchievementListResponse() : base() { }
    }
}
