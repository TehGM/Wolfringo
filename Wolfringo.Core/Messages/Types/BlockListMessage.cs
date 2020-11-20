using Newtonsoft.Json;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>A message for requesting blocked users list.</summary>
    /// <remarks>Uses <see cref="BlockListResponse"/> as response type.</remarks>
    [ResponseType(typeof(BlockListResponse))]
    public class BlockListMessage : IWolfMessage
    {
        /// <inheritdoc/>
        [JsonIgnore]
        public string EventName => MessageEventNames.SubscriberBlockList;
    }
}
