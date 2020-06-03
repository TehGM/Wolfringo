using Newtonsoft.Json;

namespace TehGM.Wolfringo.Messages
{
    public class GroupMemberUpdateEvent : IWolfMessage
    {
        public string Command => MessageCommands.GroupMemberUpdate;

        [JsonProperty("groupId")]
        public uint GroupID { get; private set; }
        [JsonProperty("subscriberId", NullValueHandling = NullValueHandling.Ignore)]
        public uint UserID { get; private set; }
        [JsonProperty("capabilities", NullValueHandling = NullValueHandling.Ignore)]
        public WolfGroupCapabilities Capabilities { get; private set; }

        [JsonConstructor]
        private GroupMemberUpdateEvent() { }
    }
}
