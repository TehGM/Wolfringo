namespace TehGM.Wolfringo.Utilities.Internal
{
    /// <summary>Provides get-only access to Wolf client caches.</summary>
    public interface IWolfClientCacheAccessor
    {
        /// <summary>Get user from cache.</summary>
        /// <param name="id">ID of the user.</param>
        /// <returns>Cached user if found; otherwise null.</returns>
        WolfUser GetCachedUser(uint id);
        /// <summary>Get group from cache.</summary>
        /// <param name="id">ID of the group.</param>
        /// <returns>Cached group if found; otherwise null.</returns>
        WolfGroup GetCachedGroup(uint id);
        /// <summary>Get group from cache.</summary>
        /// <param name="name">Name of the group.</param>
        /// <returns>Cached group if found; otherwise null.</returns>
        WolfGroup GetCachedGroup(string name);
        /// <summary>Get charm from cache.</summary>
        /// <param name="id">ID of the charm.</param>
        /// <returns>Cached charm if found; otherwise null.</returns>
        WolfCharm GetCachedCharm(uint id);
        /// <summary>Get achievement from cache.</summary>
        /// <param name="language">Language of achievement's translations.</param>
        /// <param name="id">ID of the achievement.</param>
        /// <returns>Cached achievement if found in requested language; otherwise null.</returns>
        WolfAchievement GetCachedAchievement(WolfLanguage language, uint id);
    }
}
