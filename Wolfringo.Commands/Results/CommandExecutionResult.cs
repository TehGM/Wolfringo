using System.Collections.Generic;
using System.Linq;

namespace TehGM.Wolfringo.Commands.Results
{
    /// <summary>Represents a generic command execution result.</summary>
    public struct CommandExecutionResult : ICommandResult, IMessagesCommandResult
    {
        /// <inheritdoc/>
        public bool IsSuccess { get; }
        /// <inheritdoc/>
        public IEnumerable<string> Messages { get; }

        /// <summary>Creates a new result instance.</summary>
        /// <param name="isSuccess">Whether execution was successful.</param>
        public CommandExecutionResult(bool isSuccess, IEnumerable<string> messages)
        {
            this.IsSuccess = isSuccess;
            this.Messages = messages ?? Enumerable.Empty<string>();
        }

        /// <summary>Shared success result.</summary>
        public static readonly CommandExecutionResult Success = new CommandExecutionResult(true, null);
        /// <summary>Shared failure result.</summary>
        public static readonly CommandExecutionResult Failure = new CommandExecutionResult(false, null);
    }
}
