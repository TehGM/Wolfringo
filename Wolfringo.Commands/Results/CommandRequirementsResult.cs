using System;
using System.Collections.Generic;
using System.Linq;

namespace TehGM.Wolfringo.Commands.Results
{
    /// <summary>Represents results of Requirements checks.</summary>
    public class CommandRequirementsResult : ICommandResult, IMessagesCommandResult
    {
        /// <inheritdoc/>
        [Obsolete("Use Status property instead.")]
        public bool IsSuccess => this.Status == CommandResultStatus.Success;
        /// <inheritdoc/>
        public CommandResultStatus Status { get; }
        /// <inheritdoc/>
        public IEnumerable<string> Messages { get; }

        /// <summary>Creates a new result instance.</summary>
        /// <param name="status">Status telling Command Service how to proceed.</param>
        /// <param name="messages">Set of messages to reply with.</param>
        public CommandRequirementsResult(CommandResultStatus status, IEnumerable<string> messages)
        {
            this.Status = status;
            this.Messages = messages?.Where(text => !string.IsNullOrWhiteSpace(text)) ?? Enumerable.Empty<string>();
        }

        /// <summary>Creates a success result.</summary>
        /// <param name="messages">Optional set of messages to reply with.</param>
        /// <returns>A new result instance.</returns>
        public static CommandRequirementsResult Success(IEnumerable<string> messages)
            => new CommandRequirementsResult(CommandResultStatus.Success, messages);
        /// <summary>Creates a success result.</summary>
        /// <param name="messages">Optional set of messages to reply with.</param>
        /// <returns>A new result instance.</returns>
        public static CommandRequirementsResult Success(params string[] messages)
            => Success(messages as IEnumerable<string>);

        /// <summary>Creates a failure result.</summary>
        /// <param name="messages">Optional set of messages to reply with.</param>
        /// <returns>A new result instance.</returns>
        public static CommandRequirementsResult Failure(IEnumerable<string> messages)
            => new CommandRequirementsResult(CommandResultStatus.Failure, messages);
        /// <summary>Creates a failure result.</summary>
        /// <param name="messages">Optional set of messages to reply with.</param>
        /// <returns>A new result instance.</returns>
        public static CommandRequirementsResult Failure(params string[] messages)
            => Failure(messages as IEnumerable<string>);
    }
}
