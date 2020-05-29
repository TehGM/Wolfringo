using Newtonsoft.Json;

namespace TehGM.Wolfringo.Messages
{
    public class GroupMemberLeftMessage : IWolfMessage
    {
        [JsonIgnore]
        public string Command => MessageCommands.GroupMemberDelete;

        [JsonProperty("subscriberId")]
        public uint UserID { get; private set; }
        [JsonProperty("groupId")]
        public uint GroupID { get; private set; }

        [JsonConstructor]
        private GroupMemberLeftMessage() { }
    }
}
