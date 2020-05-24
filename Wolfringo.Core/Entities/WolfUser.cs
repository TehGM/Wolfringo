using Newtonsoft.Json;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo
{
    public class WolfUser : IWolfEntity
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

        public static WolfUser FromLoginResponse(LoginResponse loginResponse)
        {
            WolfUser result = new WolfUser();
            result.ID = loginResponse.UserID;
            result.Username = loginResponse.Username;
            result.Status = loginResponse.UserStatus;
            result.Reputation = loginResponse.UserReputation;
            result.Email = loginResponse.UserEmail;
            return result;
        }
    }
}
