using System.Collections.Generic;
using System.Linq;

namespace TehGM.Wolfringo.Messages.Serialization.Internal
{
    /// <summary>Utility class with shared methods for dealing with entities that have nested/child entities.</summary>
    public static class NestedEntitiesHelper
    {
        /// <summary>Gets enumerable of achievements, with all child achievements surfaced to the top of the collection.</summary>
        /// <param name="original">The original enumerable of achievements with nested entities.</param>
        /// <returns>Enumerable of achievements, with all child achievements surfaced to the top of the collection.</returns>
        public static IEnumerable<WolfAchievement> FlattenAchievementsList(IEnumerable<WolfAchievement> original)
        {
            if (original == null)
                return original;

            HashSet<WolfAchievement> results = new HashSet<WolfAchievement>();
            AddWithChildren(ref results, original);
            return results.ToArray();

            void AddWithChildren(ref HashSet<WolfAchievement> resultsList, IEnumerable<WolfAchievement> achievements)
            {
                if (achievements?.Any() != true)
                    return;

                // add all provided first
                foreach (WolfAchievement achiv in achievements)
                    resultsList.Add(achiv);

                // recursively add children of each
                foreach (WolfAchievement achiv in achievements)
                    AddWithChildren(ref resultsList, achiv.ChildAchievements);
            }
        }
    }
}
