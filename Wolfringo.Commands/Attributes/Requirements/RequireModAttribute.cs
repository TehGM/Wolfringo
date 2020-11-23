namespace TehGM.Wolfringo.Commands
{
    /// <summary>Command requirement that checks if the user has owner, admin or mod permissions.</summary>
    /// <seealso cref="RequireOwnerAttribute"/>
    /// <seealso cref="RequireAdminAttribute"/>
    /// <seealso cref="RequireGroupPrivilegeAttribute"/>
    public class RequireModAttribute : RequireGroupPrivilegeAttribute
    {
        /// <inheritdoc/>
        public RequireModAttribute() : base(WolfGroupCapabilities.Owner | WolfGroupCapabilities.Admin | WolfGroupCapabilities.Mod) { }
    }
}
