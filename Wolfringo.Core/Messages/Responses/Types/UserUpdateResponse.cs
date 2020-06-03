using Newtonsoft.Json;

namespace TehGM.Wolfringo.Messages.Responses
{
    public class UserUpdateResponse : WolfResponse, IWolfResponse
    {
        [JsonProperty("body")]
        public WolfUser UserProfile { get; private set; }
    }
}
