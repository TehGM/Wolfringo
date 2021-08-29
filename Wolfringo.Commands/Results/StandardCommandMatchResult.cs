using System;

namespace TehGM.Wolfringo.Commands.Results
{
    /// <summary>Represents results of a check whether a Standard Command should run.</summary>
    public struct StandardCommandMatchResult : ICommandResult
    {
        /// <inheritdoc/>
        [Obsolete("Use Status property instead.")]
        public bool IsSuccess => this.Status == CommandResultStatus.Success;
        /// <summary>Found arguments.</summary>
        public string[] Arguments { get; }
        /// <inheritdoc/>
        public Exception Exception { get; }
        /// <inheritdoc/>
        public CommandResultStatus Status { get; }

        /// <summary>Creates a new result instance.</summary>
        /// <param name="isSuccess">Whether check was successful.</param>
        /// <param name="arguments">Found arguments.</param>
        /// <param name="exception">Exception that occured.</param>
        [Obsolete("Use constructor with status arg instead.")]
        public StandardCommandMatchResult(bool isSuccess, string[] arguments, Exception exception)
            : this(isSuccess ? CommandResultStatus.Success : CommandResultStatus.Skip, arguments, exception) { }

        /// <summary>Creates a new result instance.</summary>
        /// <param name="status">Status telling Command Service how to proceed.</param>
        /// <param name="arguments">Found arguments.</param>
        /// <param name="exception">Exception that occured.</param>
        public StandardCommandMatchResult(CommandResultStatus status, string[] arguments, Exception exception)
        {
            this.Status = status;
            this.Arguments = arguments;
            this.Exception = exception;
        }

        /// <summary>Shared failure result.</summary>
        public static readonly StandardCommandMatchResult Skip = new StandardCommandMatchResult(CommandResultStatus.Skip, null, null);

        /// <summary>Creates a success result.</summary>
        /// <param name="arguments">Found arguments.</param>
        public static StandardCommandMatchResult Success(string[] arguments)
            => new StandardCommandMatchResult(CommandResultStatus.Success, arguments, null);
    }
}
