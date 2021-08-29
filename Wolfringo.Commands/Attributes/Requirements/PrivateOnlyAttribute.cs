using System;
using System.Threading;
using System.Threading.Tasks;
using TehGM.Wolfringo.Commands.Attributes;
using TehGM.Wolfringo.Commands.Results;

namespace TehGM.Wolfringo.Commands
{
    /// <summary>Command requirement that checks if message is a private message.</summary>
    /// <remarks><para>Default <see cref="CommandRequirementAttribute.ErrorMessage"/> for this requirement is "(n) This command can be used in PM only.".</para></remarks>
    /// <seealso cref="GroupOnlyAttribute"/>
    public class PrivateOnlyAttribute : CommandRequirementAttribute
    {
        /// <summary>Creates a new PrivateOnly attribute instance.</summary>
        public PrivateOnlyAttribute() : base()
            => ErrorMessage = "(n) This command can be used in PM only.";

        /// <inheritdoc/>
        public override Task<ICommandResult> CheckAsync(ICommandContext context, IServiceProvider services, CancellationToken cancellationToken = default)
            => Task.FromResult(base.ResultFromBoolean(context.Message.IsGroupMessage));
    }
}
