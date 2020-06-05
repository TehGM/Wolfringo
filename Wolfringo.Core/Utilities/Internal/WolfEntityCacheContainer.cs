namespace TehGM.Wolfringo.Utilities.Internal
{
    /// <summary>Utility grouping common entity caches together.</summary>
    /// <remarks><para>This utility contains caches for entities that Wolf client is caching.</para>
    /// <para>Each cache can be separately enabled or disabled.</para></remarks>
    public class WolfEntityCacheContainer
    {
        /// <summary>Enable or disable users cache.</summary>
        public bool UsersCachingEnabled
        {
            get => _enableUsersCaching;
            set
            {
                this._enableUsersCaching = value;
                if (!value)
                    this.UsersCache.Clear();
            }
        }
        /// <summary>Enable or disable groups cache.</summary>
        public bool GroupsCachingEnabled
        {
            get => _enableGroupsCaching;
            set
            {
                this._enableGroupsCaching = value;
                if (!value)
                    this.GroupsCache.Clear();
            }
        }
        /// <summary>Enable or disable charms cache.</summary>
        public bool CharmsCachingEnabled
        {
            get => _enableCharmsCaching;
            set
            {
                this._enableCharmsCaching = value;
                if (!value)
                    this.CharmsCache.Clear();
            }
        }
        /// <summary>Enable or disable achievements cache.</summary>
        public bool AchievementsCachingEnabled
        {
            get => _enableAchievementsCaching;
            set
            {
                this._enableAchievementsCaching = value;
                if (!value)
                    this.AchievementsCache.ClearAll();
            }
        }

        private bool _enableUsersCaching;
        private bool _enableGroupsCaching;
        private bool _enableCharmsCaching;
        private bool _enableAchievementsCaching;

        /// <summary>Users cache.</summary>
        public IWolfEntityCache<WolfUser> UsersCache { get; }
        /// <summary>Groups cache.</summary>
        public IWolfEntityCache<WolfGroup> GroupsCache { get; }
        /// <summary>Charms cache.</summary>
        public IWolfEntityCache<WolfCharm> CharmsCache { get; }
        /// <summary>Achievements cache.</summary>
        public IWolfEntityCache<WolfLanguage, WolfAchievement> AchievementsCache { get; }

        /// <summary>Creates new container and contained caches.</summary>
        /// <remarks>All caches will be enabled by default.</remarks>
        public WolfEntityCacheContainer()
        {
            // init caches
            this.UsersCache = new WolfEntityCache<WolfUser>();
            this.GroupsCache = new WolfEntityCache<WolfGroup>();
            this.CharmsCache = new WolfEntityCache<WolfCharm>();
            this.AchievementsCache = new WolfEntityCache<WolfLanguage, WolfAchievement>();

            // mark caches as enabled
            this._enableUsersCaching = true;
            this._enableGroupsCaching = true;
            this._enableCharmsCaching = true;
            this._enableAchievementsCaching = true;
        }

        /// <summary>Clears all caches.</summary>
        public virtual void ClearAll()
        {
            this.UsersCache?.Clear();
            this.GroupsCache?.Clear();
            this.CharmsCache?.Clear();
            this.AchievementsCache?.ClearAll();
        }
    }
}
