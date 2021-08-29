using System;
using TehGM.Wolfringo.Commands.Results;

namespace TehGM.Wolfringo.Commands
{
    /// <summary>Represents result of any command execution or check.</summary>
    public interface ICommandResult
    {
        /// <summary>Whether execution or check was successful.</summary>
        /// <remarks>This property has been obsoleted in favour of <see cref="Status"/> and will be removed in future updates.</remarks>
        [Obsolete("Use Status property instead.")]
        bool IsSuccess { get; }
        /// <summary>Status telling commands service how to progress with the execution.</summary>
        CommandResultStatus Status { get; }
    }
}
