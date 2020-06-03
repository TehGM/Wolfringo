using Newtonsoft.Json;

namespace TehGM.Wolfringo.Messages
{
    public class UnsubscribeFromPrivateMessage : IWolfMessage
    {
        public string Command => MessageCommands.MessagePrivateUnsubscribe;

        [JsonConstructor]
        public UnsubscribeFromPrivateMessage() { }
    }
}
