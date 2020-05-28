using Newtonsoft.Json;
using TehGM.Wolfringo.Messages.Responses;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo.Messages
{
    [ResponseType(typeof(OnlineStateUpdateResponse))]
    public class OnlineStateUpdateMessage : IWolfMessage
    {
        public string Command => MessageCommands.SubscriberSettingsUpdate;

        [JsonProperty("state")]
        [JsonConverter(typeof(ObjectPropertyConverter), "state")]
        public WolfOnlineState OnlineState { get; private set; }

        public OnlineStateUpdateMessage(WolfOnlineState onlineState)
        {
            this.OnlineState = onlineState;
        }
    }
}
