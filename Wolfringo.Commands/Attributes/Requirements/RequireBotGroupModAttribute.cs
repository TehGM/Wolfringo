namespace TehGM.Wolfringo.Commands
{
    /// <summary>Command requirement that checks if the bot has owner, admin or mod permissions.</summary>
    /// <seealso cref="RequireBotGroupOwnerAttribute"/>
    /// <seealso cref="RequireBotGroupAdminAttribute"/>
    /// <seealso cref="RequireBotGroupPrivilegeAttribute"/>
    public class RequireBotGroupModAttribute : RequireBotGroupPrivilegeAttribute
    {
        /// <inheritdoc/>
        public RequireBotGroupModAttribute() : base(WolfGroupCapabilities.Owner | WolfGroupCapabilities.Admin | WolfGroupCapabilities.Mod) { }
    }
}
