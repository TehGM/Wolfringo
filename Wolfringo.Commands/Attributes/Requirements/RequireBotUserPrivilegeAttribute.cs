using System;
using System.Threading;
using System.Threading.Tasks;

namespace TehGM.Wolfringo.Commands.Attributes
{
    /// <summary>Command requirement that checks if the bot has correct privileges.</summary>
    /// <remarks><para>Default <see cref="CommandRequirementAttribute.ErrorMessage"/> for this requirement is "(n) I don't have enough user privileges to execute this command.".</para></remarks>
    public class RequireBotUserPrivilegeAttribute : CommandRequirementAttribute
    {
        /// <summary>Flags of privileges that fulfill this requirement.</summary>
        /// <remarks>Only one of the privileges has to match. For example, Volunteer | Staff matches if user is either Volunteer or Staff.</remarks>
        public WolfPrivilege Privileges { get; }

        /// <summary>Creates a new instance of command user privilege requirement.</summary>
        /// <param name="privileges">Flags of privileges that fulfill this requirement.</param>
        /// <remarks>Only one of the privileges has to match. For example, Volunteer | Staff matches if user is either Volunteer or Staff.</remarks>
        /// <seealso cref="Privileges"/>
        public RequireBotUserPrivilegeAttribute(WolfPrivilege privileges) : base()
        {
            this.Privileges = privileges;
            base.ErrorMessage = "(n) I don't have enough user privileges to execute this command.";
        }

        /// <inheritdoc/>
        public override async Task<ICommandResult> CheckAsync(ICommandContext context, IServiceProvider services, CancellationToken cancellationToken = default)
        {
            bool hasPrivileges = await RequireUserPrivilegeAttribute.CheckPrivilegeAsync(context, context.Client.CurrentUserID.Value, this.Privileges, cancellationToken).ConfigureAwait(false);
            return base.ResultFromBoolean(hasPrivileges);
        }
    }
}
