namespace TehGM.Wolfringo.Hosting
{
    public class HostedWolfClientOptions
    {
        public string ServerURL { get; set; } = WolfClient.DefaultServerURL;
        public string Device { get; set; } = WolfClient.DefaultDevice;
        public string Token { get; set; } = null;

        // login settings
        public bool AutoLogin { get; set; } = true;
        public string LoginEmail { get; set; }
        public string LoginPassword { get; set; }

        // auto-reconnection
        public bool AutoReconnect { get; set; } = true;

        // caching
        public bool UsersCachingEnabled { get; set; } = true;
        public bool GroupsCachingEnabled { get; set; } = true;
        public bool CharmsCachingEnabled { get; set; } = true;
        public bool AchievementsCachingEnabled { get; set; } = true;
    }
}
