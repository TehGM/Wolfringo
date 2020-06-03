namespace TehGM.Wolfringo.Messages
{
    public class NotificationsClearMessage : IWolfMessage
    {
        public string Command => MessageCommands.NotificationListClear;
    }
}
