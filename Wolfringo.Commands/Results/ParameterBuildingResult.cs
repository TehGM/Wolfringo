using System;
using System.Collections.Generic;
using System.Linq;

namespace TehGM.Wolfringo.Commands.Results
{
    /// <summary>Represents a a result of parameters building.</summary>
    public struct ParameterBuildingResult : ICommandResult, IMessagesCommandResult
    {
        /// <inheritdoc/>
        public bool IsSuccess { get; }
        /// <inheritdoc/>
        public Exception Exception { get; }
        /// <summary>Values for parameters that were built.</summary>
        public object[] Values { get; }
        /// <inheritdoc/>
        public IEnumerable<string> Messages { get; }

        /// <summary>Creates a new result instance.</summary>
        /// <param name="isSuccess">Whether check was successful.</param>
        /// <param name="values">Values for parameters that were built.</param>
        /// <param name="messages">Set of messages to reply with.</param>
        /// <param name="exception">An exception that has occured (if any).</param>
        public ParameterBuildingResult(bool isSuccess, object[] values, IEnumerable<string> messages, Exception exception)
        {
            this.IsSuccess = isSuccess;
            this.Values = values ?? Array.Empty<object>();
            this.Exception = exception;
            this.Messages = messages?.Where(text => !string.IsNullOrWhiteSpace(text)) ?? Enumerable.Empty<string>();
        }

        /// <summary>Creates a success result.</summary>
        /// <param name="values">Values for parameters that were built.</param>
        /// <param name="messages">Optional set of messages to reply with.</param>
        /// <returns>A new result instance.</returns>
        public static ParameterBuildingResult Success(object[] values, IEnumerable<string> messages = null)
            => new ParameterBuildingResult(true, values, messages, null);

        /// <summary>Creates a failure result.</summary>
        /// <param name="exception">An exception that has occured (if any).</param>
        /// <param name="messages">Optional set of messages to reply with.</param>
        /// <returns>A new result instance.</returns>
        public static ParameterBuildingResult Failure(Exception exception, IEnumerable<string> messages = null)
            => new ParameterBuildingResult(false, null, messages, exception);
    }
}
