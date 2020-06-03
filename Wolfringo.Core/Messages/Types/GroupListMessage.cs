using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo.Messages
{
    [ResponseType(typeof(GroupListResponse))]
    public class GroupListMessage : IWolfMessage
    {
        public string Command => MessageCommands.SubscriberGroupList;
    }
}
