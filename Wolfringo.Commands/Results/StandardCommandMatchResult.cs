using System;

namespace TehGM.Wolfringo.Commands.Results
{
    /// <summary>Represents results of a check whether a Standard Command should run.</summary>
    public struct StandardCommandMatchResult : ICommandResult
    {
        /// <inheritdoc/>
        public bool IsSuccess { get; }
        /// <summary>Found arguments.</summary>
        public string[] Arguments { get; }
        /// <inheritdoc/>
        public Exception Exception { get; }

        /// <summary>Creates a new result instance.</summary>
        /// <param name="isSuccess">Whether check was successful.</param>
        /// <param name="arguments">Found arguments.</param>
        public StandardCommandMatchResult(bool isSuccess, string[] arguments, Exception exception)
        {
            this.IsSuccess = isSuccess;
            this.Arguments = arguments;
            this.Exception = exception;
        }

        /// <summary>Shared failure result.</summary>
        public static readonly StandardCommandMatchResult Failure = new StandardCommandMatchResult(false, null, null);

        /// <summary>Creates a success result.</summary>
        /// <param name="arguments">Found arguments.</param>
        public static StandardCommandMatchResult Success(string[] arguments)
            => new StandardCommandMatchResult(true, arguments, null);
    }
}
