using TehGM.Wolfringo.Commands.Attributes;

namespace TehGM.Wolfringo.Commands
{
    /// <summary>Command requirement that checks if the bot has owner permissions.</summary>
    /// <remarks><para>Default <see cref="CommandRequirementAttribute.ErrorMessage"/> for this requirement is "(n) I need to be at least an owner to execute this command.".</para></remarks>
    /// <seealso cref="RequireBotGroupAdminAttribute"/>
    /// <seealso cref="RequireBotGroupModAttribute"/>
    /// <seealso cref="RequireBotGroupPrivilegeAttribute"/>
    public class RequireBotGroupOwnerAttribute : RequireBotGroupPrivilegeAttribute
    {
        /// <inheritdoc/>
        public RequireBotGroupOwnerAttribute() : base(WolfGroupCapabilities.Owner)
            => ErrorMessage = "(n) I need to be at least an owner to execute this command.";
    }
}
