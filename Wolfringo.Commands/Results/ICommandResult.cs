using System;

namespace TehGM.Wolfringo.Commands.Results
{
    /// <summary>Represents result of any command execution or check.</summary>
    public interface ICommandResult
    {
        /// <summary>Whether execution or check was successful.</summary>
        bool IsSuccess { get; }
        /// <summary>An exception that has occured (if any).</summary>
        Exception Exception { get; }
    }
}
