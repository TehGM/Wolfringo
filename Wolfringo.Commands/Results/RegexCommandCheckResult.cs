using System.Text.RegularExpressions;

namespace TehGM.Wolfringo.Commands.Results
{
    public struct RegexCommandCheckResult : ICommandResult
    {
        public bool IsSuccess { get; }
        public Match RegexMatch { get; }

        public RegexCommandCheckResult(bool isSuccess, Match regexMatch)
        {
            this.IsSuccess = isSuccess;
            this.RegexMatch = regexMatch;
        }

        public static readonly RegexCommandCheckResult Failure = new RegexCommandCheckResult(false, null);

        public static RegexCommandCheckResult Success(Match regexMatch)
            => new RegexCommandCheckResult(true, regexMatch);
    }
}
