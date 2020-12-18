using TehGM.Wolfringo.Commands.Attributes;

namespace TehGM.Wolfringo.Commands
{
    /// <summary>Command requirement that checks if the user has owner, admin or mod permissions.</summary>
    /// <remarks><para>Default <see cref="CommandRequirementAttribute.ErrorMessage"/> for this requirement is "(n) You need to be at least a mod to execute this command.".</para></remarks>
    /// <seealso cref="RequireGroupOwnerAttribute"/>
    /// <seealso cref="RequireGroupAdminAttribute"/>
    /// <seealso cref="RequireGroupPrivilegeAttribute"/>
    public class RequireGroupModAttribute : RequireGroupPrivilegeAttribute
    {
        /// <inheritdoc/>
        public RequireGroupModAttribute() : base(WolfGroupCapabilities.Owner | WolfGroupCapabilities.Admin | WolfGroupCapabilities.Mod)
            => ErrorMessage = "(n) You need to be at least a mod to execute this command.";
    }
}
