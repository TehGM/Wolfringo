using Newtonsoft.Json;

namespace TehGM.Wolfringo
{
    public class WolfGroupMember
    {
        [JsonProperty("id")]
        public uint UserID { get; private set; }
        [JsonProperty("capabilities")]
        public WolfGroupCapabilities Capabilities { get; private set; }

        public bool HasOwnerPrivileges => Capabilities == WolfGroupCapabilities.Owner;
        public bool HasAdminPrivileges => HasOwnerPrivileges || Capabilities == WolfGroupCapabilities.Admin;
        public bool HasModPrivileges => HasAdminPrivileges || Capabilities == WolfGroupCapabilities.Mod;

        [JsonConstructor]
        private WolfGroupMember() { }

        public WolfGroupMember(uint userID, WolfGroupCapabilities capabilities) : this()
        {
            this.UserID = userID;
            this.Capabilities = capabilities;
        }
    }
}
