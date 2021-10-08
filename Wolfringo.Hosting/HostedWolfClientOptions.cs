using System;

namespace TehGM.Wolfringo.Hosting
{
    /// <summary>Configuration options for hosted client.</summary>
    public class HostedWolfClientOptions
    {
        /// <summary>Server URL to connect to.</summary>
        /// <remarks>Defaults to <see cref="WolfClientOptions.DefaultServerURL"/>.</remarks>
        public string ServerURL { get; set; } = WolfClientOptions.DefaultServerURL;
        /// <summary>Device to use when connecting.</summary>
        /// <remarks>Defaults to <see cref="WolfClientOptions.DefaultDevice"/>.</remarks>
        public WolfDevice Device { get; set; } = WolfClientOptions.DefaultDevice;
        /// <summary>Token to use when connecting.</summary>
        /// <remarks>If not set, the client will automatically generate a valid token.</remarks>
        public string Token { get; set; } = null;

        // login settings
        /// <summary>Whether the hosted client should automatically login when connected.</summary>
        /// <remarks>Defaults to true.</remarks>
        public bool AutoLogin { get; set; } = true;
        /// <summary>Username to use as login when automatically logging in.</summary>
        public string LoginUsername { get; set; }
        /// <summary>Password to authenticate with when automatically logging in.</summary>
        public string LoginPassword { get; set; }
        /// <summary>Login type to use when automatically logging in.</summary>
        public WolfLoginType LoginType { get; set; } = WolfLoginType.Email;

        // auto-reconnection
        /// <summary>How long the client should wait before automatically reconnecting.</summary>
        /// <remarks>Defaults to 500ms.</remarks>
        public TimeSpan AutoReconnectDelay { get; set; } = TimeSpan.FromSeconds(0.5);
        /// <summary>How many times should client attempt to reconnect.</summary>
        /// <remarks>Value of 0 means reconnection will not be attempted. Negative values will be treated as infinite.
        /// <para>Defaults to 5 times.</para></remarks>
        public int AutoReconnectAttempts { get; set; } = 5;
        /// <summary>Whether the application should close on critical error.</summary>
        /// <remarks><para>Setting this to true might be useful if the application is running as a service, so it's auto-restarted.</para>
        /// <para>Defaults to false.</para></remarks>
        public bool CloseOnCriticalError { get; set; } = false;

        // caching
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

        // messages handling
        /// <summary>Whether the client should skip raising events for messages it sent.</summary>
        /// <remarks>Defaults to true.</remarks>
        public bool IgnoreOwnChatMessages { get; set; } = true;
    }
}
