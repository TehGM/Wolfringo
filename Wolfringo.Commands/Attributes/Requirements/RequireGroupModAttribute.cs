namespace TehGM.Wolfringo.Commands
{
    /// <summary>Command requirement that checks if the user has owner, admin or mod permissions.</summary>
    /// <seealso cref="RequireGroupOwnerAttribute"/>
    /// <seealso cref="RequireGroupAdminAttribute"/>
    /// <seealso cref="RequireGroupPrivilegeAttribute"/>
    public class RequireGroupModAttribute : RequireGroupPrivilegeAttribute
    {
        /// <inheritdoc/>
        public RequireGroupModAttribute() : base(WolfGroupCapabilities.Owner | WolfGroupCapabilities.Admin | WolfGroupCapabilities.Mod) { }
    }
}
