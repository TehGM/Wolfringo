using System;
using System.Threading;
using System.Threading.Tasks;
using TehGM.Wolfringo.Commands.Attributes;

namespace TehGM.Wolfringo.Commands
{
    /// <summary>Command requirement that checks if message is a group message.</summary>
    /// <remarks><para>Default <see cref="CommandRequirementAttribute.ErrorMessage"/> for this requirement is "(n) This command can be used in groups only.".</para></remarks>
    /// <seealso cref="PrivateOnlyAttribute"/>
    public class GroupOnlyAttribute : CommandRequirementAttribute
    {
        /// <summary>Creates a new GroupOnly attribute instance.</summary>
        public GroupOnlyAttribute() : base() 
            => ErrorMessage = "(n) This command can be used in groups only.";

        /// <inheritdoc/>
        public override Task<bool> CheckAsync(ICommandContext context, IServiceProvider services, CancellationToken cancellationToken = default)
            => Task.FromResult<bool>(context.Message.IsGroupMessage);
    }
}
