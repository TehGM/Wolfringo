namespace TehGM.Wolfringo.Commands
{
    /// <summary>Command requirement that checks if the bot has owner permissions.</summary>
    /// <seealso cref="RequireBotGroupAdminAttribute"/>
    /// <seealso cref="RequireBotGroupModAttribute"/>
    /// <seealso cref="RequireBotGroupPrivilegeAttribute"/>
    public class RequireBotGroupOwnerAttribute : RequireBotGroupPrivilegeAttribute
    {
        /// <inheritdoc/>
        public RequireBotGroupOwnerAttribute() : base(WolfGroupCapabilities.Owner) { }
    }
}
