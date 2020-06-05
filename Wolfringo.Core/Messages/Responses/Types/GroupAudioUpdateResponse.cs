using Newtonsoft.Json;

namespace TehGM.Wolfringo.Messages.Responses
{
    /// <summary>Wolf server's response for <see cref="GroupAudioUpdateMessage"/>.</summary>
    public class GroupAudioUpdateResponse : WolfResponse, IWolfResponse
    {
        /// <summary>New audio config.</summary>
        [JsonProperty("body")]
        public WolfGroup.WolfGroupAudioConfig AudioConfig { get; private set; }
    }
}
