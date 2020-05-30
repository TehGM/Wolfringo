namespace TehGM.Wolfringo.Utilities.Internal
{
    public class WolfEntityCacheContainer
    {
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

        private bool _enableUsersCaching;
        private bool _enableGroupsCaching;
        private bool _enableCharmsCaching;

        public IWolfEntityCache<WolfUser> UsersCache { get; }
        public IWolfEntityCache<WolfGroup> GroupsCache { get; }
        public IWolfEntityCache<WolfCharm> CharmsCache { get; }

        public WolfEntityCacheContainer()
        {
            // init caches
            this.UsersCache = new WolfEntityCache<WolfUser>();
            this.GroupsCache = new WolfEntityCache<WolfGroup>();
            this.CharmsCache = new WolfEntityCache<WolfCharm>();

            // mark caches as enabled
            this._enableUsersCaching = true;
            this._enableGroupsCaching = true;
            this._enableCharmsCaching = true;
        }

        public virtual void ClearAll()
        {
            this.UsersCache.Clear();
            this.GroupsCache.Clear();
            this.CharmsCache.Clear();
        }
    }
}
