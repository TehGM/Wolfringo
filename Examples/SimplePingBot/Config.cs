using Newtonsoft.Json;
using System.IO;
using System.Text.Json;

namespace TehGM.Wolfringo.Examples.SimplePingBot
{
    class Config
    {
        [JsonProperty("username")]
        public string Username { get; private set; }
        [JsonProperty("password")]
        public string Password { get; private set; }

        public static Config Load()
            => JsonConvert.DeserializeObject<Config>(File.ReadAllText("appsecrets.json"));
    }
}
