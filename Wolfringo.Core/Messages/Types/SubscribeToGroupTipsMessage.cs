using Newtonsoft.Json;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>A message for subscribing to tips for group messages.</summary>
    /// <remarks>Uses <see cref="EntitiesSubscribeResponse"/> as response type.</remarks>
    [ResponseType(typeof(EntitiesSubscribeResponse))]
    public class SubscribeToGroupTipsMessage : IWolfMessage
    {
        /// <inheritdoc/>
        /// <remarks>Equals to <see cref="MessageEventNames.TipGroupSubscribe"/>.</remarks>
        [JsonIgnore]
        public string EventName => MessageEventNames.TipGroupSubscribe;

        /// <summary>Creates a message instance.</summary>
        [JsonConstructor]
        public SubscribeToGroupTipsMessage() { }
    }
}
