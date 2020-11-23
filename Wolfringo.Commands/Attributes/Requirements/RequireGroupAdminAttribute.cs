namespace TehGM.Wolfringo.Commands
{
    /// <summary>Command requirement that checks if the user has owner or admin permissions.</summary>
    /// <seealso cref="RequireGroupOwnerAttribute"/>
    /// <seealso cref="RequireGroupModAttribute"/>
    /// <seealso cref="RequireGroupPrivilegeAttribute"/>
    public class RequireGroupAdminAttribute : RequireGroupPrivilegeAttribute
    {
        /// <inheritdoc/>
        public RequireGroupAdminAttribute() : base(WolfGroupCapabilities.Owner | WolfGroupCapabilities.Admin) { }
    }
}
