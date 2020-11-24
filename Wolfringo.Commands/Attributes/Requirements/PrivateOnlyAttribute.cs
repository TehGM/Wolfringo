using System;
using System.Threading;
using System.Threading.Tasks;

namespace TehGM.Wolfringo.Commands
{
    /// <summary>Command requirement that checks if message is a private message.</summary>
    /// <remarks><para>Default <see cref="CommandRequirementAttribute.ErrorMessage"/> for this requirement is "(n) This command can be used in PM only.".</para></remarks>
    /// <seealso cref="GroupOnlyAttribute"/>
    public class PrivateOnlyAttribute : CommandRequirementAttribute
    {
        public PrivateOnlyAttribute() : base()
            => ErrorMessage = "(n) This command can be used in PM only.";

        /// <inheritdoc/>
        public override Task<bool> RunAsync(ICommandContext context, IServiceProvider services, CancellationToken cancellationToken = default)
            => Task.FromResult<bool>(!context.Message.IsGroupMessage);
    }
}
