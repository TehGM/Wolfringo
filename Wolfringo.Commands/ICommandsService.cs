using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TehGM.Wolfringo.Commands.Initialization;

namespace TehGM.Wolfringo.Commands
{
    /// <summary>A service that deals with commands loading, initialization and execution.</summary>
    public interface ICommandsService
    {
        /// <summary>Descriptors of all commands loaded to this commands service.</summary>
        IEnumerable<ICommandInstanceDescriptor> Commands { get; }
        /// <summary>Starts the Command Service.</summary>
        /// <param name="cancellationToken">Cancellation token to cancel loading with.</param>
        Task StartAsync(CancellationToken cancellationToken = default);
        /// <summary>Executes commands against a command context.</summary>
        /// <param name="context">Context to execute commands with.</param>
        /// <param name="cancellationToken">Cancellation token to cancel execution with.</param>
        /// <returns>Result of the execution.</returns>
        Task<ICommandResult> ExecuteAsync(ICommandContext context, CancellationToken cancellationToken = default);
    }
}
