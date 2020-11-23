using System;
using System.Threading;
using System.Threading.Tasks;

namespace TehGM.Wolfringo.Commands
{
    /// <summary>Command requirement that checks if user has no more reputation level and percentage than specified.</summary>
    public class RequireMaximumReputationAttribute : CommandRequirementAttribute
    {
        /// <summary>Maximum reputation the user needs to have.</summary>
        /// <seealso cref="WolfUser.Reputation"/>
        public double MaximumReputation { get; }

        /// <summary>Creates a new instance of command reputation requirement.</summary>
        /// <param name="maximumReputation">Maximum reputation the user needs to have.</param>
        public RequireMaximumReputationAttribute(double maximumReputation)
        {
            if (maximumReputation < 0)
                throw new ArgumentException("Reputation value cannot be negative", nameof(maximumReputation));
            this.MaximumReputation = maximumReputation;
        }

        /// <inheritdoc/>
        public override async Task<bool> RunAsync(ICommandContext context, CancellationToken cancellationToken = default)
        {
            WolfUser user = await context.GetSenderAsync(cancellationToken).ConfigureAwait(false);
            return user.Reputation <= this.MaximumReputation;
        }
    }
}
