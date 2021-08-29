using System;
using System.Collections.Generic;
using System.Linq;

namespace TehGM.Wolfringo.Commands.Results
{
    /// <summary>Represents a a result of parameters building.</summary>
    public struct ParameterBuildingResult : ICommandResult, IMessagesCommandResult
    {
        /// <inheritdoc/>
        [Obsolete("Use Status property instead.")]
        public bool IsSuccess => this.Status == CommandResultStatus.Success;
        /// <summary>Values for parameters that were built.</summary>
        public object[] Values { get; }
        /// <inheritdoc/>
        public CommandResultStatus Status { get; }
        /// <inheritdoc/>
        public IEnumerable<string> Messages { get; }

        /// <summary>Creates a new result instance.</summary>
        /// <param name="isSuccess">Whether check was successful.</param>
        /// <param name="values">Values for parameters that were built.</param>
        /// <param name="messages">Set of messages to reply with.</param>
        public ParameterBuildingResult(bool isSuccess, object[] values, IEnumerable<string> messages)
        {
            this.Status = isSuccess ? CommandResultStatus.Success : CommandResultStatus.Failure;
            this.Values = values ?? Array.Empty<object>();
            this.Messages = messages?.Where(text => !string.IsNullOrWhiteSpace(text)) ?? Enumerable.Empty<string>();
        }

        /// <summary>Creates a success result.</summary>
        /// <param name="values">Values for parameters that were built.</param>
        /// <param name="messages">Optional set of messages to reply with.</param>
        /// <returns>A new result instance.</returns>
        public static ParameterBuildingResult Success(object[] values, IEnumerable<string> messages = null)
            => new ParameterBuildingResult(true, values, messages);

        /// <summary>Creates a failure result.</summary>
        /// <param name="messages">Optional set of messages to reply with.</param>
        /// <returns>A new result instance.</returns>
        public static ParameterBuildingResult Failure(IEnumerable<string> messages = null)
            => new ParameterBuildingResult(false, null, messages);
    }
}
