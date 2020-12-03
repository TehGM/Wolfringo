using TehGM.Wolfringo.Commands.Attributes;

namespace TehGM.Wolfringo.Commands
{
    /// <summary>Command requirement that checks if the user has a Staff tag.</summary>
    /// <remarks><para>Default <see cref="CommandRequirementAttribute.ErrorMessage"/> for this requirement is "(n) You need to be a Staff to execute this command.".</para></remarks>
    public class RequireUserStaffAttribute : RequireUserPrivilegeAttribute
    {
        /// <inheritdoc/>
        public RequireUserStaffAttribute() : base(WolfPrivilege.Staff)
            => ErrorMessage = "(n) You need to be a Staff to execute this command.";
    }
}
