using System;
using System.Threading;
using System.Threading.Tasks;
using TehGM.Wolfringo.Commands.Attributes;

namespace TehGM.Wolfringo.Commands
{
    /// <summary>Command requirement that checks if user has at least specified reputation level and percentage.</summary>
    /// <remarks><para>Default <see cref="CommandRequirementAttribute.ErrorMessage"/> for this requirement is "(n) Your reputation is too low to execute this command.".</para></remarks>
    public class RequireMinimumReputationAttribute : CommandRequirementAttribute
    {
        /// <summary>Minimum reputation the user needs to have.</summary>
        /// <seealso cref="WolfUser.Reputation"/>
        public double MinimumReputation { get; }

        /// <summary>Creates a new instance of command reputation requirement.</summary>
        /// <param name="minimumReputation">Minimum reputation the user needs to have.</param>
        public RequireMinimumReputationAttribute(double minimumReputation) : base()
        {
            if (minimumReputation < 0)
                throw new ArgumentException("Reputation value cannot be negative", nameof(minimumReputation));
            this.MinimumReputation = minimumReputation;
            base.ErrorMessage = "(n) Your reputation is too low to execute this command.";
        }

        /// <inheritdoc/>
        public override async Task<ICommandResult> CheckAsync(ICommandContext context, IServiceProvider services, CancellationToken cancellationToken = default)
        {
            WolfUser user = await context.GetSenderAsync(cancellationToken).ConfigureAwait(false);
            return base.ResultFromBoolean(user.Reputation >= this.MinimumReputation);
        }
    }
}
