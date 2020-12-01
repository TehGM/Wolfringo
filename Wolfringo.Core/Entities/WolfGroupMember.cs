﻿using Newtonsoft.Json;

namespace TehGM.Wolfringo
{
    /// <summary>Member of a group.</summary>
    public class WolfGroupMember
    {
        /// <summary>User ID.</summary>
        [JsonProperty("id")]
        public uint UserID { get; private set; }
        /// <summary>User's permissions.</summary>
        [JsonProperty("capabilities")]
        public WolfGroupCapabilities Capabilities { get; private set; }

        /// <summary>Does the member have owner privileges?</summary>
        public bool HasOwnerPrivileges => Capabilities == WolfGroupCapabilities.Owner;
        /// <summary>Does the member have admin or greater privileges?</summary>
        public bool HasAdminPrivileges => HasOwnerPrivileges || Capabilities == WolfGroupCapabilities.Admin;
        /// <summary>Does the member have mod or greater privileges?</summary>
        public bool HasModPrivileges => HasAdminPrivileges || Capabilities == WolfGroupCapabilities.Mod;

        /// <summary>Creates a new instance of WOLF group member object.</summary>
        [JsonConstructor]
        protected WolfGroupMember() { }

        /// <summary>Creates a new instance of WOLF group member object.</summary>
        /// <param name="userID">ID of the user.</param>
        /// <param name="capabilities">User's group permissions.</param>
        public WolfGroupMember(uint userID, WolfGroupCapabilities capabilities) : this()
        {
            this.UserID = userID;
            this.Capabilities = capabilities;
        }
    }
}
