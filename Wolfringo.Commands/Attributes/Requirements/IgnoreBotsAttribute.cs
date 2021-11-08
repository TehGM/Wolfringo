using System;
using System.Threading;
using System.Threading.Tasks;
using TehGM.Wolfringo.Commands.Attributes;
using TehGM.Wolfringo.Commands.Results;

namespace TehGM.Wolfringo.Commands
{
    /// <summary>Command requirement that will ignore all official bot messages.</summary>
    /// <remarks>This requirement works by checking for User's <see cref="WolfUser.Privileges"/> to see if it has <see cref="WolfPrivilege.Bot"/> privilege.</remarks>
    public class IgnoreBotsAttribute : CommandRequirementAttribute
    {
        /// <inheritdoc/>
        public override async Task<ICommandResult> CheckAsync(ICommandContext context, IServiceProvider services, CancellationToken cancellationToken = default)
        {
            bool isBot = await RequireUserPrivilegeAttribute.CheckPrivilegeAsync(context, context.Message.SenderID.Value, WolfPrivilege.Bot, cancellationToken).ConfigureAwait(false);
            return base.ResultFromBoolean(isBot);
        }
    }
}
