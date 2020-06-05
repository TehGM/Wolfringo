namespace TehGM.Wolfringo
{
    /// <summary>Online state of the user.</summary>
    public enum WolfOnlineState
    {
        // values borrowed from https://github.com/dewwalters/Wolf.Net/blob/master/Wolf.Net/Enums/OnlineState.cs
        /// <summary>User is offline.</summary>
        Offline = 0,
        /// <summary>User is onlin.</summary>
        Online = 1,
        /// <summary>User is away.</summary>
        Away = 2,
        /// <summary>User is logged in, but appearing as offline.</summary>
        Invisible = 3,
        /// <summary>User is busy.</summary>
        Busy = 5,
        /// <summary>User is idle.</summary>
        Idle = 9
    }
}
