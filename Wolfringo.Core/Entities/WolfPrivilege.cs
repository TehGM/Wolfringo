using System;

namespace TehGM.Wolfringo
{
    /// <summary><see cref="WolfUser"/>'s global account privileges.</summary>
    [Flags]
    public enum WolfPrivilege : long
    {
        // values based on https://github.com/calico-crusade/WolfLive.Api/blob/main/WolfLive.Api/Models/Types/PrivilegeType.cs
        /// <summary>User is enabled.</summary>
        User = 1,
        /// <summary>User has Select 1 tag.</summary>
        Select1 = 1 << 4,
        /// <summary>User has Elite 1 tag.</summary>
        Elite1 = 1 << 6,
        /// <summary>User is a volunteer.</summary>
        Volunteer = 1 << 9,
        /// <summary>User has Select 2 tag.</summary>
        Select2 = 1 << 10,
        /// <summary>User is a tester.</summary>
        Tester = 1 << 11,
        /// <summary>User is a staff.</summary>
        Staff = 1 << 12,
        /// <summary>User has Elite 2 tag.</summary>
        Elite2 = 1 << 17,
        /// <summary>User is a pest.</summary>
        Pest = 1 << 18,
        /// <summary>User's email is verified.</summary>
        EmailVerified = 1 << 19,
        /// <summary>User has a premium account.</summary>
        Premium = 1 << 20,
        /// <summary>User is a VIP.</summary>
        VIP = 1 << 21,
        /// <summary>User has Elite 3 tag.</summary>
        Elite3 = 1 << 22,
        /// <summary>User can admin users.</summary>
        UserAdmin = 1 << 24,
        /// <summary>User can admin groups.</summary>
        GroupAdmin = 1 << 25,
        /// <summary>User is a bot.</summary>
        Bot = 1 << 26,
        /// <summary>User is an agent.</summary>
        Agent = 1 << 28,
        /// <summary>User is an entertainer.</summary>
        Entertainer = 1 << 29,
        /// <summary>User is barred</summary>
        Barred = 1 << 30
    }
}
