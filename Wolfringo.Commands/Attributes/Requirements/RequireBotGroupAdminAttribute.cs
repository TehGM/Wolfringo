using TehGM.Wolfringo.Commands.Attributes;

namespace TehGM.Wolfringo.Commands
{
    /// <summary>Command requirement that checks if the bot has owner or admin permissions.</summary>
    /// <remarks><para>Default <see cref="CommandRequirementAttribute.ErrorMessage"/> for this requirement is "(n) I need to be at least an admin to execute this command.".</para></remarks>
    /// <seealso cref="RequireBotGroupOwnerAttribute"/>
    /// <seealso cref="RequireBotGroupModAttribute"/>
    /// <seealso cref="RequireBotGroupPrivilegeAttribute"/>
    public class RequireBotGroupAdminAttribute : RequireBotGroupPrivilegeAttribute
    {
        /// <inheritdoc/>
        public RequireBotGroupAdminAttribute() : base(WolfGroupCapabilities.Owner | WolfGroupCapabilities.Admin)
            => ErrorMessage = "(n) I need to be at least an admin to execute this command.";
    }
}
