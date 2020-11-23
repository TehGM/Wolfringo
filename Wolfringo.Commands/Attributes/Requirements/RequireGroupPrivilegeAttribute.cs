using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TehGM.Wolfringo.Messages;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo.Commands
{
    /// <summary>Command requirement that checks if the user has correct group permissions.</summary>
    /// <seealso cref="RequireOwnerAttribute"/>
    /// <seealso cref="RequireAdminAttribute"/>
    /// <seealso cref="RequireModAttribute"/>
    public class RequireGroupPrivilegeAttribute : CommandRequirementAttribute
    {
        /// <summary>Flags of privileges that fulfill this requirement.</summary>
        /// <remarks>Only one of the privileges has to match. For example, Owner | Admin matches if user is either Owner or Admin.</remarks>
        public WolfGroupCapabilities Privileges { get; }
        /// <summary>Whether this requirement should be ignored in private messages.</summary>
        /// <remarks><para>If this is set to true, <see cref="RunAsync(ICommandContext, CancellationToken)"/> will always return true for private messages.
        /// This is useful if the command should work normally in PM, even if in group it requires permissions.<br/>
        /// If this is set to false, <see cref="RunAsync(ICommandContext, CancellationToken)"/> will always return false for private messages.
        /// This will make command group-only.</para>
        /// <para>Defaults to false.</para></remarks>
        public bool IgnoreInPrivate { get; set; } = false;

        /// <summary>Creates a new instance of command group privilege requirement.</summary>
        /// <param name="privileges">Flags of privileges that fulfill this requirement.</param>
        /// <remarks>Only one of the privileges has to match. For example, Owner | Admin matches if user is either Owner or Admin.</remarks>
        /// <seealso cref="Privileges"/>
        public RequireGroupPrivilegeAttribute(WolfGroupCapabilities privileges)
        {
            this.Privileges = privileges;
        }

        /// <inheritdoc/>
        public override async Task<bool> RunAsync(ICommandContext context, CancellationToken cancellationToken = default)
        {
            if (!context.Message.IsGroupMessage)
                return IgnoreInPrivate;

            uint id = context.Message.SenderID.Value;
            WolfGroup group = await context.GetRecipientAsync<WolfGroup>(cancellationToken).ConfigureAwait(false);
            if (!group.Members.TryGetValue(id, out WolfGroupMember member) && group.Members?.Any() != true)
            {
                GroupMembersListResponse membersResponse = await context.Client.SendAsync<GroupMembersListResponse>(
                    new GroupMembersListMessage(group.ID), cancellationToken).ConfigureAwait(false);
                member = membersResponse.GroupMembers.FirstOrDefault(u => u.UserID == id);
            }
            if (member == null)
                return false;
            return (this.Privileges & member.Capabilities) == member.Capabilities;
        }
    }
}
