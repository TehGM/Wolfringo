namespace TehGM.Wolfringo.Commands.Results
{
    /// <summary>Represent a status of command result.</summary>
    public enum CommandResultStatus
    {
        /// <summary>Step was successful, continue.</summary>
        Success,
        /// <summary>Step has failed, and the command execution should be aborted.</summary>
        Failure,
        /// <summary>Step has failed, but execution should not be aborted - try running the next command.</summary>
        Skip
    }
}
