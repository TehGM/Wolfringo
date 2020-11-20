namespace TehGM.Wolfringo.Commands.Results
{
    public struct CommandExecutionResult : ICommandResult
    {
        public bool IsSuccess { get; }

        private CommandExecutionResult(bool isSuccess)
        {
            this.IsSuccess = isSuccess;
        }

        public static readonly CommandExecutionResult Success = new CommandExecutionResult(true);
        public static readonly CommandExecutionResult Failure = new CommandExecutionResult(false);
    }
}
