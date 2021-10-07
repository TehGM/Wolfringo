using System;
using System.Text.RegularExpressions;
using TehGM.Wolfringo.Commands.Parsing;

namespace TehGM.Wolfringo.Commands.Results
{
    /// <summary>Represents results of a check whether a Regex Command should run.</summary>
    public class RegexCommandMatchResult : ICommandResult
    {
        /// <inheritdoc/>
        [Obsolete("Use Status property instead.")]
        public bool IsSuccess => this.Status == CommandResultStatus.Success;
        /// <summary>Result of regex match.</summary>
        public Match RegexMatch { get; }
        /// <inheritdoc/>
        public CommandResultStatus Status { get; }
        /// <summary>Options for command context, with command's overrides applied.</summary>
        public CommandContextOptions Options { get; }

        /// <summary>Creates a new result instance.</summary>
        /// <param name="status">Status telling Command Service how to proceed.</param>
        /// <param name="regexMatch">Result of regex match.</param>
        /// <param name="options">Options for command context, with command's overrides applied.</param>
        public RegexCommandMatchResult(CommandResultStatus status, Match regexMatch, CommandContextOptions options)
        {
            this.Status = status;
            this.RegexMatch = regexMatch;
            this.Options = options;
        }

        /// <summary>Shared failure result.</summary>
        public static readonly RegexCommandMatchResult Skip = new RegexCommandMatchResult(CommandResultStatus.Skip, null, null);

        /// <summary>Creates a success result.</summary>
        /// <param name="regexMatch">Result of regex match.</param>
        /// <param name="options">Options for command context, with command's overrides applied.</param>
        public static RegexCommandMatchResult Success(Match regexMatch, CommandContextOptions options)
            => new RegexCommandMatchResult(CommandResultStatus.Success, regexMatch, options);
    }
}
