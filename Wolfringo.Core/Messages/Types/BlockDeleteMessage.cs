using Newtonsoft.Json;

namespace TehGM.Wolfringo.Messages
{
    public class BlockDeleteMessage : IWolfMessage
    {
        public string Command => MessageCommands.SubscriberBlockDelete;

        [JsonProperty("id")]
        public uint UserID { get; private set; }

        [JsonConstructor]
        private BlockDeleteMessage() { }

        public BlockDeleteMessage(uint userId) : this()
        {
            this.UserID = userId;
        }
    }
}
