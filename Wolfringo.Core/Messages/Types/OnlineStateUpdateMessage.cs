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
        /// <remarks>Equals to <see cref="MessageEventNames.SubscriberSettingsUpdate"/>.</remarks>
        [JsonIgnore]
        public string EventName => MessageEventNames.SubscriberSettingsUpdate;

        /// <summary>New online state.</summary>
        [JsonProperty("state")]
        [JsonConverter(typeof(ObjectPropertyConverter), "state")]
        public WolfOnlineState OnlineState { get; private set; }

        /// <summary>Creates a message instance.</summary>
        [JsonConstructor]
        protected OnlineStateUpdateMessage() { }

        /// <summary>Creates a message instance.</summary>
        /// <param name="onlineState">Online state to set.</param>
        public OnlineStateUpdateMessage(WolfOnlineState onlineState) : this()
        {
            this.OnlineState = onlineState;
        }
    }
}
