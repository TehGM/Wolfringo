namespace TehGM.Wolfringo
{
    /// <summary>Member permissions within a group.</summary>
    public enum WolfGroupCapabilities
    {
        // values borrowed from https://github.com/dewwalters/Wolf.Net/blob/master/Wolf.Net/Enums/Capabilities.cs
        NonGroupRequest = -2,
        /// <summary>Normal user without permissions.</summary>
        User = 0,
        /// <summary>Group admin.</summary>
        Admin = 1,
        /// <summary>Group mod.</summary>
        Mod = 2,
        /// <summary>Banned in the group.</summary>
        Banned = 4,
        /// <summary>Silenced in the group.</summary>
        Silenced = 8,
        /// <summary>Not a member of the group.</summary>
        NotMember = 16,
        /// <summary>Group owner.</summary>
        Owner = 32
    }
}
