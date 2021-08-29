using System;

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

        /// <summary>Creates a new result instance.</summary>
        /// <param name="isSuccess">Whether check was successful.</param>
        /// <param name="arguments">Found arguments.</param>
        [Obsolete("Use constructor with status arg instead.")]
        public StandardCommandMatchResult(bool isSuccess, string[] arguments)
            : this(isSuccess ? CommandResultStatus.Success : CommandResultStatus.Skip, arguments) { }

        /// <summary>Creates a new result instance.</summary>
        /// <param name="status">Status telling Command Service how to proceed.</param>
        /// <param name="arguments">Found arguments.</param>
        public StandardCommandMatchResult(CommandResultStatus status, string[] arguments)
        {
            this.Status = status;
            this.Arguments = arguments;
        }

        /// <summary>Shared failure result.</summary>
        public static readonly StandardCommandMatchResult Skip = new StandardCommandMatchResult(CommandResultStatus.Skip, null);

        /// <summary>Creates a success result.</summary>
        /// <param name="arguments">Found arguments.</param>
        public static StandardCommandMatchResult Success(string[] arguments)
            => new StandardCommandMatchResult(CommandResultStatus.Success, arguments);
    }
}
