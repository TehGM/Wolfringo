using System;
using System.Text.RegularExpressions;

namespace TehGM.Wolfringo.Commands.Results
{
    /// <summary>Represents results of a check whether a Regex Command should run.</summary>
    public struct RegexCommandMatchResult : ICommandResult
    {
        /// <inheritdoc/>
        [Obsolete("Use Status property instead.")]
        public bool IsSuccess => this.Status == CommandResultStatus.Success;
        /// <summary>Result of regex match.</summary>
        public Match RegexMatch { get; }
        /// <inheritdoc/>
        public Exception Exception { get; }
        /// <inheritdoc/>
        public CommandResultStatus Status { get; }

        /// <summary>Creates a new result instance.</summary>
        /// <param name="isSuccess">Whether check was successful.</param>
        /// <param name="regexMatch">Result of regex match.</param>
        /// <param name="exception">Exception that occured.</param>
        [Obsolete("Use constructor with status arg instead.")]
        public RegexCommandMatchResult(bool isSuccess, Match regexMatch, Exception exception)
            : this(isSuccess ? CommandResultStatus.Success : CommandResultStatus.Skip, regexMatch, exception) { }

        /// <summary>Creates a new result instance.</summary>
        /// <param name="status">Status telling Command Service how to proceed.</param>
        /// <param name="regexMatch">Result of regex match.</param>
        /// <param name="exception">Exception that occured.</param>
        public RegexCommandMatchResult(CommandResultStatus status, Match regexMatch, Exception exception)
        {
            this.Status = status;
            this.RegexMatch = regexMatch;
            this.Exception = exception;
        }

        /// <summary>Shared failure result.</summary>
        public static readonly RegexCommandMatchResult Skip = new RegexCommandMatchResult(CommandResultStatus.Skip, null, null);

        /// <summary>Creates a success result.</summary>
        /// <param name="regexMatch">Result of regex match.</param>
        public static RegexCommandMatchResult Success(Match regexMatch)
            => new RegexCommandMatchResult(CommandResultStatus.Success, regexMatch, null);
    }
}
