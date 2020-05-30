using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo.Messages
{
    [ResponseType(typeof(ListUserGroupsResponse))]
    public class ListUserGroupsMessage : IWolfMessage
    {
        public string Command => MessageCommands.SubscriberGroupList;
    }
}
