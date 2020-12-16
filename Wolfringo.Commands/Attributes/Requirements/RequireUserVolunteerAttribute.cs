using TehGM.Wolfringo.Commands.Attributes;

namespace TehGM.Wolfringo.Commands
{
    /// <summary>Command requirement that checks if the user has a Volunteer tag.</summary>
    /// <remarks><para>Default <see cref="CommandRequirementAttribute.ErrorMessage"/> for this requirement is "(n) You need to be a Volunteer to execute this command.".</para></remarks>
    public class RequireUserVolunteerAttribute : RequireUserPrivilegeAttribute
    {
        /// <inheritdoc/>
        public RequireUserVolunteerAttribute() : base(WolfPrivilege.Staff)
            => ErrorMessage = "(n) You need to be a Volunteer to execute this command.";
    }
}