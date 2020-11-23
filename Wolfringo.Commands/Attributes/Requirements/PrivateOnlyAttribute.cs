using System.Threading;
using System.Threading.Tasks;

namespace TehGM.Wolfringo.Commands
{
    /// <summary>Command requirement that checks if message is a private message.</summary>
    /// <seealso cref="GroupOnlyAttribute"/>
    public class PrivateOnlyAttribute : CommandRequirementAttribute
    {
        /// <inheritdoc/>
        public override Task<bool> RunAsync(ICommandContext context, CancellationToken cancellationToken = default)
            => Task.FromResult<bool>(!context.Message.IsGroupMessage);
    }
}
