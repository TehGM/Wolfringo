using Newtonsoft.Json;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo.Messages.Responses
{
    public class OnlineStateUpdateResponse : WolfResponse, IWolfResponse
    {
        [JsonProperty("state")]
        [JsonConverter(typeof(ValueOrPropertyConverter), "state")]
        public WolfOnlineState OnlineState { get; private set; }
    }
}
