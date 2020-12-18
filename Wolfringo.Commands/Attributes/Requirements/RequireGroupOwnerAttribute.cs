using TehGM.Wolfringo.Commands.Attributes;

namespace TehGM.Wolfringo.Commands
{
    /// <summary>Command requirement that checks if the user has owner permissions.</summary>
    /// <remarks><para>Default <see cref="CommandRequirementAttribute.ErrorMessage"/> for this requirement is "(n) You need to be at least an owner to execute this command.".</para></remarks>
    /// <seealso cref="RequireGroupAdminAttribute"/>
    /// <seealso cref="RequireGroupModAttribute"/>
    /// <seealso cref="RequireGroupPrivilegeAttribute"/>
    public class RequireGroupOwnerAttribute : RequireGroupPrivilegeAttribute
    {
        /// <inheritdoc/>
        public RequireGroupOwnerAttribute() : base(WolfGroupCapabilities.Owner)
            => ErrorMessage = "(n) You need to be an owner to execute this command.";
    }
}
