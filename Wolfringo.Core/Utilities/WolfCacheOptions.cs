namespace TehGM.Wolfringo.Utilities
{
    /// <summary>Options for the entity cache used by the client.</summary>
    public class WolfCacheOptions
    {
        /// <summary>Whether users caching should be enabled.</summary>
        /// <remarks>Defaults to true.</remarks>
        public bool UsersCachingEnabled { get; set; } = true;
        /// <summary>Whether groups caching should be enabled.</summary>
        /// <remarks>Defaults to true.</remarks>
        public bool GroupsCachingEnabled { get; set; } = true;
        /// <summary>Whether charms caching should be enabled.</summary>
        /// <remarks>Defaults to true.</remarks>
        public bool CharmsCachingEnabled { get; set; } = true;
        /// <summary>Whether achievements caching should be enabled.</summary>
        /// <remarks>Defaults to true.</remarks>
        public bool AchievementsCachingEnabled { get; set; } = true;
    }
}
