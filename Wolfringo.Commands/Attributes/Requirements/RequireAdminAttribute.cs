namespace TehGM.Wolfringo.Commands
{
    /// <summary>Command requirement that checks if the user has owner or admin permissions.</summary>
    /// <seealso cref="RequireOwnerAttribute"/>
    /// <seealso cref="RequireModAttribute"/>
    /// <seealso cref="RequireGroupPrivilegeAttribute"/>
    public class RequireAdminAttribute : RequireGroupPrivilegeAttribute
    {
        /// <inheritdoc/>
        public RequireAdminAttribute() : base(WolfGroupCapabilities.Owner | WolfGroupCapabilities.Admin) { }
    }
}
