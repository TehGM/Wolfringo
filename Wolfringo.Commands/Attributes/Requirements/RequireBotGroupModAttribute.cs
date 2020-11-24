namespace TehGM.Wolfringo.Commands
{
    /// <summary>Command requirement that checks if the bot has owner, admin or mod permissions.</summary>
    /// <remarks><para>Default <see cref="CommandRequirementAttribute.ErrorMessage"/> for this requirement is "(n) I need to be at least a mod to execute this command.".</para></remarks>
    /// <seealso cref="RequireBotGroupOwnerAttribute"/>
    /// <seealso cref="RequireBotGroupAdminAttribute"/>
    /// <seealso cref="RequireBotGroupPrivilegeAttribute"/>
    public class RequireBotGroupModAttribute : RequireBotGroupPrivilegeAttribute
    {
        /// <inheritdoc/>
        public RequireBotGroupModAttribute() : base(WolfGroupCapabilities.Owner | WolfGroupCapabilities.Admin | WolfGroupCapabilities.Mod)
            => ErrorMessage = "(n) I need to be at least a mod to execute this command.";
    }
}
