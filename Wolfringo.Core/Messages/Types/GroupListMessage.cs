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
        [JsonIgnore]
        public string Command => MessageCommands.SubscriberGroupList;
    }
}
