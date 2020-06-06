using Newtonsoft.Json;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo.Messages.Responses
{
    /// <summary>Wolf server's response for <see cref="OnlineStateUpdateMessage"/>.</summary>
    public class OnlineStateUpdateResponse : WolfResponse, IWolfResponse
    {
        /// <summary>New online state.</summary>
        [JsonProperty("state")]
        [JsonConverter(typeof(ValueOrPropertyConverter), "state")]
        public WolfOnlineState OnlineState { get; private set; }

        [JsonConstructor]
        protected OnlineStateUpdateResponse() : base() { }
    }
}
