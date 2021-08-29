using System;
using System.Threading;
using System.Threading.Tasks;

namespace TehGM.Wolfringo.Commands.Attributes
{
    /// <summary>Command requirement that checks if the bot has correct group permissions.</summary>
    /// <remarks><para>Default <see cref="CommandRequirementAttribute.ErrorMessage"/> for this requirement is "(n) I don't have enough group privileges to execute this command.".</para></remarks>
    /// <seealso cref="RequireBotGroupOwnerAttribute"/>
    /// <seealso cref="RequireBotGroupAdminAttribute"/>
    /// <seealso cref="RequireBotGroupModAttribute"/>
    public class RequireBotGroupPrivilegeAttribute : CommandRequirementAttribute
    {
        /// <summary>Flags of privileges that fulfill this requirement.</summary>
        /// <remarks>Only one of the privileges has to match. For example, Owner | Admin matches if bot is either Owner or Admin.</remarks>
        public WolfGroupCapabilities Privileges { get; }
        /// <summary>Whether this requirement should be ignored in private messages.</summary>
        /// <remarks><para>If this is set to true, <see cref="CheckAsync(ICommandContext, IServiceProvider, CancellationToken)"/> will always return true for private messages.
        /// This is useful if the command should work normally in PM, even if in group it requires permissions.<br/>
        /// If this is set to false, <see cref="CheckAsync(ICommandContext, IServiceProvider, CancellationToken)"/> will always return false for private messages.
        /// This will make command group-only.</para>
        /// <para>Defaults to false.</para></remarks>
        public bool IgnoreInPrivate { get; set; } = false;

        /// <summary>Creates a new instance of command group privilege requirement.</summary>
        /// <param name="privileges">Flags of privileges that fulfill this requirement.</param>
        /// <remarks>Only one of the privileges has to match. For example, Owner | Admin matches if bot is either Owner or Admin.</remarks>
        /// <seealso cref="Privileges"/>
        public RequireBotGroupPrivilegeAttribute(WolfGroupCapabilities privileges) : base()
        {
            this.Privileges = privileges;
            base.ErrorMessage = "(n) I don't have enough group privileges to execute this command.";
        }

        /// <inheritdoc/>
        public override async Task<ICommandResult> CheckAsync(ICommandContext context, IServiceProvider services, CancellationToken cancellationToken = default)
        {
            if (!context.Message.IsGroupMessage)
                return base.ResultFromBoolean(this.IgnoreInPrivate);

            bool hasPrivileges = await RequireGroupPrivilegeAttribute.CheckPrivilegeAsync(context, context.Client.CurrentUserID.Value, this.Privileges, cancellationToken).ConfigureAwait(false);
            return base.ResultFromBoolean(hasPrivileges);
        }
    }
}
