using TehGM.Wolfringo.Commands.Attributes;

namespace TehGM.Wolfringo.Commands
{
    /// <summary>Command requirement that checks if the user has an Entertainer tag.</summary>
    /// <remarks><para>Default <see cref="CommandRequirementAttribute.ErrorMessage"/> for this requirement is "(n) You need to be an Entertainer to execute this command.".</para></remarks>
    public class RequireUserEntertainerAttribute : RequireUserPrivilegeAttribute
    {
        /// <inheritdoc/>
        public RequireUserEntertainerAttribute() : base(WolfPrivilege.Entertainer)
            => ErrorMessage = "(n) You need to be an Entertainer to execute this command.";
    }
}
