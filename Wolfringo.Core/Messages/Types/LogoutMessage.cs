namespace TehGM.Wolfringo.Messages
{
    public class LogoutMessage : IWolfMessage
    {
        public string Command => MessageCommands.SecurityLogout;
    }
}
