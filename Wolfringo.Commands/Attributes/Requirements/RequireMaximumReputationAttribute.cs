using System;
using System.Threading;
using System.Threading.Tasks;

namespace TehGM.Wolfringo.Commands
{
    /// <summary>Command requirement that checks if user has no more reputation level and percentage than specified.</summary>
    /// <remarks><para>Default <see cref="CommandRequirementAttribute.ErrorMessage"/> for this requirement is "(n) Your reputation is too high to execute this command.".</para></remarks>
    public class RequireMaximumReputationAttribute : CommandRequirementAttribute
    {
        /// <summary>Maximum reputation the user needs to have.</summary>
        /// <seealso cref="WolfUser.Reputation"/>
        public double MaximumReputation { get; }

        /// <summary>Creates a new instance of command reputation requirement.</summary>
        /// <param name="maximumReputation">Maximum reputation the user needs to have.</param>
        public RequireMaximumReputationAttribute(double maximumReputation) : base()
        {
            if (maximumReputation < 0)
                throw new ArgumentException("Reputation value cannot be negative", nameof(maximumReputation));
            this.MaximumReputation = maximumReputation;
            base.ErrorMessage = "(n) Your reputation is too high to execute this command.";
        }

        /// <inheritdoc/>
        public override async Task<bool> CheckAsync(ICommandContext context, IServiceProvider services, CancellationToken cancellationToken = default)
        {
            WolfUser user = await context.GetSenderAsync(cancellationToken).ConfigureAwait(false);
            return user.Reputation <= this.MaximumReputation;
        }
    }
}
