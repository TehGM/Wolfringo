namespace TehGM.Wolfringo.Messages
{
    /// <summary>Represents a WOLF protocol event that signals group privileges updates.</summary>
    /// <remarks>Internally used for caching.</remarks>
    /// <seealso cref="GroupMemberPrivilegedAddEvent"/>
    /// <seealso cref="GroupMemberPrivilegedUpdateEvent"/>
    /// <seealso cref="GroupMemberPrivilegedDeleteEvent"/>
    public interface IGroupMemberPrivilegedEvent : IWolfMessage
    {
        /// <summary>ID of the group.</summary>
        uint GroupID { get; }
        /// <summary>Updated member's ID.</summary>
        uint UserID { get; }
        /// <summary>Updated member's permissions.</summary>
        WolfGroupCapabilities Capabilities { get; }
    }
}
