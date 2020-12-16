using Newtonsoft.Json;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>A message for requesting groups list.</summary>
    /// <remarks>Uses <see cref="GroupListResponse"/> as response type.</remarks>
    [ResponseType(typeof(GroupListResponse))]
    public class GroupListMessage : IWolfMessage
    {
        /// <inheritdoc/>
        /// <remarks>Equals to <see cref="MessageEventNames.SubscriberGroupList"/>.</remarks>
        [JsonIgnore]
        public string EventName => MessageEventNames.SubscriberGroupList;
    }
}
