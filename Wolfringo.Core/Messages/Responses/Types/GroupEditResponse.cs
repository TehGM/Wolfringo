using Newtonsoft.Json;

namespace TehGM.Wolfringo.Messages.Responses
{
    public class GroupEditResponse : WolfResponse, IWolfResponse
    {
        [JsonProperty("body")]
        public WolfGroup GroupProfile { get; private set; }
    }
}
