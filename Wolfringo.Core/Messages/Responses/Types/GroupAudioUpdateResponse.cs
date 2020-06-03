using Newtonsoft.Json;

namespace TehGM.Wolfringo.Messages.Responses
{
    public class GroupAudioUpdateResponse : WolfResponse, IWolfResponse
    {
        [JsonProperty("body")]
        public WolfGroup.WolfGroupAudioConfig AudioConfig { get; private set; }
    }
}
