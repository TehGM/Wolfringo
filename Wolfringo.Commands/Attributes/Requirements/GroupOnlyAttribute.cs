using System.Threading;
using System.Threading.Tasks;

namespace TehGM.Wolfringo.Commands
{
    /// <summary>Command requirement that checks if message is a group message.</summary>
    /// <seealso cref="PrivateOnlyAttribute"/>
    public class GroupOnlyAttribute : CommandRequirementAttribute
    {
        /// <inheritdoc/>
        public override Task<bool> RunAsync(ICommandContext context, CancellationToken cancellationToken = default)
            => Task.FromResult<bool>(context.Message.IsGroupMessage);
    }
}
