using Newtonsoft.Json;

namespace TehGM.Wolfringo
{
    public class WolfUser
    {
        [JsonProperty("id")]
        public uint ID { get; protected set; }
        [JsonProperty("nickname")]
        public string Username { get; protected set; }
        [JsonProperty("status")]
        public string Status { get; protected set; }
        [JsonProperty("reputation")]
        public double Reputation { get; protected set; }
        [JsonProperty("email")]
        public string Email { get; protected set; }
    }
}
