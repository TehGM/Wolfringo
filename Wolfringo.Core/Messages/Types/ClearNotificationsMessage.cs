namespace TehGM.Wolfringo.Messages
{
    public class ClearNotificationsMessage : IWolfMessage
    {
        public string Command => MessageCommands.NotificationListClear;
    }
}
