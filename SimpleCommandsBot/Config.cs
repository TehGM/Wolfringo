using Newtonsoft.Json;
using System.IO;
using System.Text.Json;

namespace TehGM.Wolfringo.Examples.SimpleCommandsBot
{
    /// <summary>Example simple configuration class.</summary>
    /// <remarks>Pushing application secrets to a repository is a bad idea, therefore it's recommended to have a configuration class that reads configuration file instead.</remarks>
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
