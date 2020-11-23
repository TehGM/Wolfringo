namespace TehGM.Wolfringo.Commands
{
    /// <summary>Command requirement that checks if the user has owner permissions.</summary>
    /// <seealso cref="RequireGroupAdminAttribute"/>
    /// <seealso cref="RequireGroupModAttribute"/>
    /// <seealso cref="RequireGroupPrivilegeAttribute"/>
    public class RequireGroupOwnerAttribute : RequireGroupPrivilegeAttribute
    {
        /// <inheritdoc/>
        public RequireGroupOwnerAttribute() : base(WolfGroupCapabilities.Owner) { }
    }
}
