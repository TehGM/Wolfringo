using Newtonsoft.Json;
using TehGM.Wolfringo.Messages.Responses;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>A message for changing online state.</summary>
    /// <remarks>Uses <see cref="OnlineStateUpdateResponse"/> as response type.</remarks>
    [ResponseType(typeof(OnlineStateUpdateResponse))]
    public class OnlineStateUpdateMessage : IWolfMessage
    {
        /// <inheritdoc/>
        [JsonIgnore]
        public string Command => MessageCommands.SubscriberSettingsUpdate;

        /// <summary>New online state.</summary>
        [JsonProperty("state")]
        [JsonConverter(typeof(ObjectPropertyConverter), "state")]
        public WolfOnlineState OnlineState { get; private set; }

        [JsonConstructor]
        private OnlineStateUpdateMessage() { }

        /// <summary>Creates a message instance.</summary>
        /// <param name="onlineState">Online state to set.</param>
        public OnlineStateUpdateMessage(WolfOnlineState onlineState) : this()
        {
            this.OnlineState = onlineState;
        }
    }
}
