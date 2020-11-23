using System;
using System.Threading;
using System.Threading.Tasks;

namespace TehGM.Wolfringo.Commands
{
    /// <summary>Represents a command instance.</summary>
    public interface ICommandInstance
    {
        /// <summary>Determines whether the command should execute.</summary>
        /// <param name="context">Context of the command.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the checks.</param>
        /// <returns>Result of the check.</returns>
        Task<ICommandResult> CheckMatchAsync(ICommandContext context, CancellationToken cancellationToken = default);
        /// <summary>Executes the command.</summary>
        /// <param name="context">Context of the command.</param>
        /// <param name="services">Services provider for injecting parameters into command method.</param>
        /// <param name="matchResult">Result of the pre-run check.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the execution.</param>
        /// <returns>Result of the execution.</returns>
        Task<ICommandResult> ExecuteAsync(ICommandContext context, IServiceProvider services, ICommandResult matchResult, CancellationToken cancellationToken = default);
    }
}
