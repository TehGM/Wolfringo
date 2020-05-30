using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TehGM.Wolfringo.Messages
{
    public class GroupJoinMessage : IWolfMessage
    {
        public string Command => MessageCommands.GroupMemberAdd;

        [JsonProperty("groupId")]
        public uint GroupID { get; private set; }

        // sending only
        [JsonProperty("password", NullValueHandling = NullValueHandling.Ignore)]
        public string Password { get; private set; }
        // receiving only
        [JsonProperty("subscriberId", NullValueHandling = NullValueHandling.Ignore)]
        public uint? UserID { get; private set; }
        [JsonProperty("capabilities", NullValueHandling = NullValueHandling.Ignore)]
        public WolfGroupCapabilities? Capabilities { get; private set; }

        [JsonConstructor]
        private GroupJoinMessage() { }

        public GroupJoinMessage(uint groupID, string password = null) : this()
        {
            this.GroupID = groupID;
            this.Password = password ?? string.Empty;
        }
    }
}
