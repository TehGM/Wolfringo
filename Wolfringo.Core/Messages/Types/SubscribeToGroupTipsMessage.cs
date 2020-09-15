using Newtonsoft.Json;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>A message for subscribing to tips for group messages.</summary>
    [ResponseType(typeof(EntitiesSubscribeResponse))]
    public class SubscribeToGroupTipsMessage : IWolfMessage
    {
        public string Command => MessageCommands.TipGroupSubscribe;

        [JsonConstructor]
        public SubscribeToGroupTipsMessage() { }
    }
}
