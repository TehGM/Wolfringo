using Newtonsoft.Json;

namespace TehGM.Wolfringo.Messages
{
    public class GroupLeaveMessage : IWolfMessage
    {
        [JsonIgnore]
        public string Command => MessageCommands.GroupMemberDelete;

        [JsonProperty("groupId")]
        public uint GroupID { get; private set; }
        // receiving only
        [JsonProperty("subscriberId", NullValueHandling = NullValueHandling.Ignore)]
        public uint? UserID { get; private set; }

        [JsonConstructor]
        private GroupLeaveMessage() { }

        public GroupLeaveMessage(uint groupID) : this()
        {
            this.GroupID = groupID;
        }
    }
}
