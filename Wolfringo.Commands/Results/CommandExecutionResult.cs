using System;
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
        /// <inheritdoc/>
        public Exception Exception { get; }

        /// <summary>Creates a new result instance.</summary>
        /// <param name="isSuccess">Whether execution was successful.</param>
        /// <param name="messages">Error messages to reply with.</param>
        /// <param name="exception">Exception that resulted in given status.</param>
        public CommandExecutionResult(bool isSuccess, IEnumerable<string> messages, Exception exception)
        {
            this.IsSuccess = isSuccess;
            this.Messages = messages?.Where(text => !string.IsNullOrWhiteSpace(text)) ?? Enumerable.Empty<string>();
            this.Exception = exception;
        }

        /// <summary>Shared success result.</summary>
        public static readonly CommandExecutionResult Success = new CommandExecutionResult(true, null, null);
        /// <summary>Shared failure result.</summary>
        public static readonly CommandExecutionResult Failure = new CommandExecutionResult(false, null, null);

        /// <summary>Creates a failure result with exception and optional messages.</summary>
        /// <param name="exception">Exception that is cause of a failure.</param>
        /// <param name="messages">Messages to respond with.</param>
        /// <returns>A new failure result.</returns>
        public static CommandExecutionResult FromException(Exception exception, IEnumerable<string> messages = null)
            => new CommandExecutionResult(false, messages, exception);
    }
}
