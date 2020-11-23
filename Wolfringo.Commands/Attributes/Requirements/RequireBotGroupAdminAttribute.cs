namespace TehGM.Wolfringo.Commands
{
    /// <summary>Command requirement that checks if the bot has owner or admin permissions.</summary>
    /// <seealso cref="RequireBotGroupOwnerAttribute"/>
    /// <seealso cref="RequireBotGroupModAttribute"/>
    /// <seealso cref="RequireBotGroupPrivilegeAttribute"/>
    public class RequireBotGroupAdminAttribute : RequireBotGroupPrivilegeAttribute
    {
        /// <inheritdoc/>
        public RequireBotGroupAdminAttribute() : base(WolfGroupCapabilities.Owner | WolfGroupCapabilities.Admin) { }
    }
}
