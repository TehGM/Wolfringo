using System.Collections.Generic;

namespace TehGM.Wolfringo.Commands.Results
{
    /// <summary>Represents result of any command execution or check that has messages to reply with.</summary>
    public interface IMessagesCommandResult : ICommandResult
    {
        /// <summary>Set of messages to reply with.</summary>
        IEnumerable<string> Messages { get; }
    }
}
