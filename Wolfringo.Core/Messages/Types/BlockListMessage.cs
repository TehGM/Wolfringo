using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo.Messages
{
    [ResponseType(typeof(BlockListResponse))]
    public class BlockListMessage : IWolfMessage
    {
        public string Command => MessageCommands.SubscriberBlockList;
    }
}
