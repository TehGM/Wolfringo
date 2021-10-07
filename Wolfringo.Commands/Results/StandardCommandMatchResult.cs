using System;
using TehGM.Wolfringo.Commands.Parsing;

namespace TehGM.Wolfringo.Commands.Results
{
    /// <summary>Represents results of a check whether a Standard Command should run.</summary>
    public class StandardCommandMatchResult : ICommandResult
    {
        /// <inheritdoc/>
        [Obsolete("Use Status property instead.")]
        public bool IsSuccess => this.Status == CommandResultStatus.Success;
        /// <summary>Found arguments.</summary>
        public string[] Arguments { get; }
        /// <inheritdoc/>
        public CommandResultStatus Status { get; }
        /// <summary>Options for command context, with command's overrides applied.</summary>
        public CommandContextOptions Options { get; }

        /// <summary>Creates a new result instance.</summary>
        /// <param name="status">Status telling Command Service how to proceed.</param>
        /// <param name="arguments">Found arguments.</param>
        /// <param name="options">Options for command context, with command's overrides applied.</param>
        public StandardCommandMatchResult(CommandResultStatus status, string[] arguments, CommandContextOptions options)
        {
            this.Status = status;
            this.Arguments = arguments;
        }

        /// <summary>Shared failure result.</summary>
        public static readonly StandardCommandMatchResult Skip = new StandardCommandMatchResult(CommandResultStatus.Skip, null, null);

        /// <summary>Creates a success result.</summary>
        /// <param name="arguments">Found arguments.</param>
        /// <param name="options">Options for command context, with command's overrides applied.</param>
        public static StandardCommandMatchResult Success(string[] arguments, CommandContextOptions options)
            => new StandardCommandMatchResult(CommandResultStatus.Success, arguments, options);
    }
}
