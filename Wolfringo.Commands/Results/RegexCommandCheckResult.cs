using System.Text.RegularExpressions;

namespace TehGM.Wolfringo.Commands.Results
{
    /// <summary>Represents results of a check whether a Regex Command should run.</summary>
    public struct RegexCommandCheckResult : ICommandResult
    {
        /// <inheritdoc/>
        public bool IsSuccess { get; }
        /// <summary>Result of regex match.</summary>
        public Match RegexMatch { get; }

        /// <summary>Creates a new result instance.</summary>
        /// <param name="isSuccess">Whether check was successful.</param>
        /// <param name="regexMatch">Result of regex match.</param>
        public RegexCommandCheckResult(bool isSuccess, Match regexMatch)
        {
            this.IsSuccess = isSuccess;
            this.RegexMatch = regexMatch;
        }

        /// <summary>Shared failure result.</summary>
        public static readonly RegexCommandCheckResult Failure = new RegexCommandCheckResult(false, null);

        /// <summary>Creates a success result.</summary>
        /// <param name="regexMatch">Result of regex match.</param>
        public static RegexCommandCheckResult Success(Match regexMatch)
            => new RegexCommandCheckResult(true, regexMatch);
    }
}
