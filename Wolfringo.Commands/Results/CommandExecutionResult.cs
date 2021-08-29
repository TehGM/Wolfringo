using System;
using System.Collections.Generic;
using System.Linq;

namespace TehGM.Wolfringo.Commands.Results
{
    /// <summary>Represents a generic command execution result.</summary>
    public class CommandExecutionResult : ICommandResult, IMessagesCommandResult
    {
        /// <inheritdoc/>
        [Obsolete("Use Status property instead.")]
        public bool IsSuccess => this.Status == CommandResultStatus.Success;
        /// <inheritdoc/>
        public IEnumerable<string> Messages { get; }
        /// <inheritdoc/>
        public Exception Exception { get; }
        /// <inheritdoc/>
        public CommandResultStatus Status { get; }

        /// <summary>Creates a new result instance.</summary>
        /// <remarks>This constructor has been obsoleted in favour of <see cref="Status"/> approach and will be removed in future updates.</remarks>
        /// <param name="status">The execution status.</param>
        /// <param name="messages">Error messages to reply with.</param>
        /// <param name="exception">Exception that resulted in given status.</param>
        public CommandExecutionResult(CommandResultStatus status, IEnumerable<string> messages, Exception exception)
        {
            this.Status = status;
            this.Messages = messages?.Where(text => !string.IsNullOrWhiteSpace(text)) ?? Enumerable.Empty<string>();
            this.Exception = exception;
        }

        /// <summary>Creates a new result instance.</summary>
        /// <remarks>This constructor has been obsoleted in favour of <see cref="Status"/> approach and will be removed in future updates.</remarks>
        /// <param name="isSuccess">Whether execution was successful.</param>
        /// <param name="messages">Error messages to reply with.</param>
        /// <param name="exception">Exception that resulted in given status.</param>
        [Obsolete("Use constructor with status arg instead.")]
        public CommandExecutionResult(bool isSuccess, IEnumerable<string> messages, Exception exception)
            : this(isSuccess ? CommandResultStatus.Success : CommandResultStatus.Failure, messages, exception) { }

        /// <summary>Shared success result.</summary>
        public static readonly CommandExecutionResult Success = new CommandExecutionResult(CommandResultStatus.Success, null, null);
        /// <summary>Shared failure result.</summary>
        public static readonly CommandExecutionResult Failure = new CommandExecutionResult(CommandResultStatus.Failure, null, null);
        /// <summary>Shared skip result.</summary>
        public static readonly CommandExecutionResult Skip = new CommandExecutionResult(CommandResultStatus.Skip, null, null);

        /// <summary>Creates a failure result with exception and optional messages.</summary>
        /// <param name="exception">Exception that is cause of a failure.</param>
        /// <param name="messages">Messages to respond with.</param>
        /// <returns>A new failure result.</returns>
        public static CommandExecutionResult FromException(Exception exception, IEnumerable<string> messages = null)
            => new CommandExecutionResult(CommandResultStatus.Failure, messages, exception);
    }
}
