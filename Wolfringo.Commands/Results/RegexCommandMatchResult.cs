using System;
using System.Text.RegularExpressions;

namespace TehGM.Wolfringo.Commands.Results
{
    /// <summary>Represents results of a check whether a Regex Command should run.</summary>
    public struct RegexCommandMatchResult : ICommandResult
    {
        /// <inheritdoc/>
        public bool IsSuccess { get; }
        /// <summary>Result of regex match.</summary>
        public Match RegexMatch { get; }
        /// <inheritdoc/>
        public Exception Exception { get; }

        /// <summary>Creates a new result instance.</summary>
        /// <param name="isSuccess">Whether check was successful.</param>
        /// <param name="regexMatch">Result of regex match.</param>
        public RegexCommandMatchResult(bool isSuccess, Match regexMatch, Exception exception)
        {
            this.IsSuccess = isSuccess;
            this.RegexMatch = regexMatch;
            this.Exception = exception;
        }

        /// <summary>Shared failure result.</summary>
        public static readonly RegexCommandMatchResult Failure = new RegexCommandMatchResult(false, null, null);

        /// <summary>Creates a success result.</summary>
        /// <param name="regexMatch">Result of regex match.</param>
        public static RegexCommandMatchResult Success(Match regexMatch)
            => new RegexCommandMatchResult(true, regexMatch, null);
    }
}
