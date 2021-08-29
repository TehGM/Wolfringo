using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TehGM.Wolfringo.Messages;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo.Commands.Attributes
{
    /// <summary>Command requirement that checks if the user has correct privileges.</summary>
    /// <remarks><para>Default <see cref="CommandRequirementAttribute.ErrorMessage"/> for this requirement is "(n) You don't have enough user privileges to execute this command.".</para></remarks>
    public class RequireUserPrivilegeAttribute : CommandRequirementAttribute
    {
        /// <summary>Flags of privileges that fulfill this requirement.</summary>
        /// <remarks>Only one of the privileges has to match. For example, Volunteer | Staff matches if user is either Volunteer or Staff.</remarks>
        public WolfPrivilege Privileges { get; }

        /// <summary>Creates a new instance of command user privilege requirement.</summary>
        /// <param name="privileges">Flags of privileges that fulfill this requirement.</param>
        /// <remarks>Only one of the privileges has to match. For example, Volunteer | Staff matches if user is either Volunteer or Staff.</remarks>
        /// <seealso cref="Privileges"/>
        public RequireUserPrivilegeAttribute(WolfPrivilege privileges) : base()
        {
            this.Privileges = privileges;
            base.ErrorMessage = "(n) You don't have enough user privileges to execute this command.";
        }

        /// <inheritdoc/>
        public override async Task<ICommandResult> CheckAsync(ICommandContext context, IServiceProvider services, CancellationToken cancellationToken = default)
        {
            bool hasPrivilege = await CheckPrivilegeAsync(context, context.Message.SenderID.Value, this.Privileges, cancellationToken).ConfigureAwait(false);
            return base.ResultFromBoolean(hasPrivilege);
        }

        /// <summary>Checks if user has a specified privilege.</summary>
        /// <param name="context">Context of the command execution.</param>
        /// <param name="userID">ID of the user.</param>
        /// <param name="privileges">Required privileges flags.</param>
        /// <param name="cancellationToken">Cancellation token for cancelling the task.</param>
        /// <returns>True if user has at least one of specified privileges; otherwise false.</returns>
        public static async Task<bool> CheckPrivilegeAsync(ICommandContext context, uint userID, WolfPrivilege privileges, CancellationToken cancellationToken = default)
        {
            UserProfileResponse response = await context.Client.SendAsync<UserProfileResponse>(
                new UserProfileMessage(new uint[] { userID }, true, true), cancellationToken).ConfigureAwait(false);
            WolfUser user = response?.UserProfiles?.FirstOrDefault(u => u.ID == userID);
            if (user == null)
                return false;
            return (privileges & user.Privileges) != 0;
        }
    }
}
