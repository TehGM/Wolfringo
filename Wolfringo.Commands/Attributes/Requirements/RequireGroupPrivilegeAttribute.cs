using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TehGM.Wolfringo.Messages;
using TehGM.Wolfringo.Messages.Responses;
using TehGM.Wolfringo.Utilities.Internal;

namespace TehGM.Wolfringo.Commands.Attributes
{
    /// <summary>Command requirement that checks if the user has correct group permissions.</summary>
    /// <remarks><para>Default <see cref="CommandRequirementAttribute.ErrorMessage"/> for this requirement is "(n) You don't have enough group privileges to execute this command.".</para></remarks>
    /// <seealso cref="RequireGroupOwnerAttribute"/>
    /// <seealso cref="RequireGroupAdminAttribute"/>
    /// <seealso cref="RequireGroupModAttribute"/>
    public class RequireGroupPrivilegeAttribute : CommandRequirementAttribute
    {
        /// <summary>Flags of privileges that fulfill this requirement.</summary>
        /// <remarks>Only one of the privileges has to match. For example, Owner | Admin matches if user is either Owner or Admin.</remarks>
        public WolfGroupCapabilities Privileges { get; }
        /// <summary>Whether this requirement should be ignored in private messages.</summary>
        /// <remarks><para>If this is set to true, <see cref="CheckAsync(ICommandContext, IServiceProvider, CancellationToken)"/> will always return true for private messages.
        /// This is useful if the command should work normally in PM, even if in group it requires permissions.<br/>
        /// If this is set to false, <see cref="CheckAsync(ICommandContext, IServiceProvider, CancellationToken)"/>  will always return false for private messages.
        /// This will make command group-only.</para>
        /// <para>Defaults to false.</para></remarks>
        public bool IgnoreInPrivate { get; set; } = false;

        /// <summary>Creates a new instance of command group privilege requirement.</summary>
        /// <param name="privileges">Flags of privileges that fulfill this requirement.</param>
        /// <remarks>Only one of the privileges has to match. For example, Owner | Admin matches if user is either Owner or Admin.</remarks>
        /// <seealso cref="Privileges"/>
        public RequireGroupPrivilegeAttribute(WolfGroupCapabilities privileges) : base()
        {
            this.Privileges = privileges;
            base.ErrorMessage = "(n) You don't have enough group privileges to execute this command.";
        }

        /// <inheritdoc/>
        public override async Task<ICommandResult> CheckAsync(ICommandContext context, IServiceProvider services, CancellationToken cancellationToken = default)
        {
            if (!context.Message.IsGroupMessage)
                return base.ResultFromBoolean(this.IgnoreInPrivate);

            bool hasPrivileges = await CheckPrivilegeAsync(context, context.Message.SenderID.Value, this.Privileges, cancellationToken).ConfigureAwait(false);
            return base.ResultFromBoolean(hasPrivileges);
        }

        /// <summary>Checks if user has a specified privilege.</summary>
        /// <param name="context">Context of the command execution.</param>
        /// <param name="userID">ID of the user.</param>
        /// <param name="privileges">Required privileges flags.</param>
        /// <param name="cancellationToken">Cancellation token for cancelling the task.</param>
        /// <returns>True if user has at least one of specified privileges; otherwise false.</returns>
        public static async Task<bool> CheckPrivilegeAsync(ICommandContext context, uint userID, WolfGroupCapabilities privileges, CancellationToken cancellationToken = default)
        {
            WolfGroup group = await context.GetRecipientAsync<WolfGroup>(cancellationToken).ConfigureAwait(false);
            await context.Client.RevalidateGroupMembersAsync(group, cancellationToken).ConfigureAwait(false);

            return group.Members.TryGetValue(userID, out WolfGroupMember member) && (privileges & member.Capabilities) == member.Capabilities;
        }
    }
}
