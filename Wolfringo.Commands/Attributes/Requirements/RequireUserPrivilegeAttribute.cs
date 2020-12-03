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
        public override Task<bool> CheckAsync(ICommandContext context, IServiceProvider services, CancellationToken cancellationToken = default)
            => CheckPrivilegeAsync(context, context.Message.SenderID.Value, this.Privileges, cancellationToken);

        internal static async Task<bool> CheckPrivilegeAsync(ICommandContext context, uint userID, WolfPrivilege privileges, CancellationToken cancellationToken = default)
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
