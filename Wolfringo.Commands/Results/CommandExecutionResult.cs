namespace TehGM.Wolfringo.Commands.Results
{
    /// <summary>Represents a generic command execution result.</summary>
    public struct CommandExecutionResult : ICommandResult
    {
        /// <inheritdoc/>
        public bool IsSuccess { get; }

        /// <summary>Creates a new result instance.</summary>
        /// <param name="isSuccess">Whether execution was successful.</param>
        private CommandExecutionResult(bool isSuccess)
        {
            this.IsSuccess = isSuccess;
        }

        /// <summary>Shared success result.</summary>
        public static readonly CommandExecutionResult Success = new CommandExecutionResult(true);
        /// <summary>Shared failure result.</summary>
        public static readonly CommandExecutionResult Failure = new CommandExecutionResult(false);
    }
}
