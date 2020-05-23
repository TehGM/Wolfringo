using TehGM.Wolfringo.Utilities;

namespace TehGM.Wolfringo
{
    public class WolfClientOptions
    {
        public const string DefaultServerURL = "wss://v3-rc.palringo.com:3051";
        public const string DefaultDevice = "bot";

        public string ServerURL { get; set; } = DefaultServerURL;
        public string Device { get; set; } = DefaultDevice;
        public string Token { get; set; } = null;
    }
}
