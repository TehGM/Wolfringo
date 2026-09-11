namespace TehGM.Wolfringo
{
    /// <summary>Represents options for <see cref="WolfClient"/>.</summary>
    public class WolfClientOptions
    {
        /// <summary>Default Wolf server URL.</summary>
        public const string DefaultServerURL = "wss://v3.palringo.com:3051";
        /// <summary>Pre-release Wolf server URL.</summary>
        public const string BetaServerURL = "wss://v3-rc.palringo.com:3051";
        /// <summary>Default device to pass to the server when connecting.</summary>
        public const WolfDevice DefaultDevice = WolfDevice.Bot;

        /// <summary>API Key to connect with. Must match the account that will be used to connect.</summary>
        public string ApiKey { get; set; }
        /// <summary>WOLF server URL to connect to.</summary>
        public string ServerURL { get; set; } = DefaultServerURL;
        /// <summary>Device to connect as.</summary>
        public WolfDevice Device { get; set; } = DefaultDevice;
        /// <summary>Whether the client should skip raising events for messages it sent.</summary>
        /// <remarks>Defaults to true.</remarks>
        public bool IgnoreOwnChatMessages { get; set; } = true;
    }
}
