using Newtonsoft.Json;

namespace TehGM.Wolfringo.Messages
{
    public class GroupMemberJoinedMessage : IWolfMessage
    {
        public string Command => MessageCommands.GroupMemberAdd;

        [JsonProperty("subscriberId")]
        public uint UserID { get; private set; }
        [JsonProperty("groupId")]
        public uint GroupID { get; private set; }
        [JsonProperty("capabilities")]
        public WolfGroupCapabilities Capabilities { get; private set; }

        [JsonConstructor]
        private GroupMemberJoinedMessage() { }

        public WolfGroupMember GetGroupMember()
            => new WolfGroupMember(this.UserID, this.Capabilities);
    }
}
