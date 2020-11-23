namespace TehGM.Wolfringo.Commands
{
    /// <summary>Command requirement that checks if the user has owner permissions.</summary>
    /// <seealso cref="RequireAdminAttribute"/>
    /// <seealso cref="RequireModAttribute"/>
    /// <seealso cref="RequireGroupPrivilegeAttribute"/>
    public class RequireOwnerAttribute : RequireGroupPrivilegeAttribute
    {
        /// <inheritdoc/>
        public RequireOwnerAttribute() : base(WolfGroupCapabilities.Owner) { }
    }
}
