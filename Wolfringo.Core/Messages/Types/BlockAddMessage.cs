using Newtonsoft.Json;

namespace TehGM.Wolfringo.Messages
{
    public class BlockAddMessage : IWolfMessage
    {
        public string Command => MessageCommands.SubscriberBlockAdd;

        [JsonProperty("id")]
        public uint UserID { get; private set; }

        [JsonConstructor]
        private BlockAddMessage() { }

        public BlockAddMessage(uint userId) : this()
        {
            this.UserID = userId;
        }
    }
}
