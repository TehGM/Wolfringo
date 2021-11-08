using System;
using System.Threading;
using System.Threading.Tasks;

namespace TehGM.Wolfringo.Commands.Initialization
{
    /// <summary>Represents a command instance.</summary>
    public interface ICommandInstance
    {
        /// <summary>Determines whether the command should execute.</summary>
        /// <param name="context">Context of the command.</param>
        /// <param name="services">Service provider that can be used during the checks.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the checks.</param>
        /// <returns>Result of the check.</returns>
        Task<ICommandResult> CheckMatchAsync(ICommandContext context, IServiceProvider services, CancellationToken cancellationToken = default);
        /// <summary>Executes the command.</summary>
        /// <param name="context">Context of the command.</param>
        /// <param name="services">Services provider for injecting parameters into command method.</param>
        /// <param name="matchResult">Result of the pre-run check.</param>
        /// <param name="handler">Handler object to execute the command method in.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the execution.</param>
        /// <returns>Result of the execution.</returns>
        Task<ICommandResult> ExecuteAsync(ICommandContext context, IServiceProvider services, ICommandResult matchResult, object handler, CancellationToken cancellationToken = default);
    }
}
