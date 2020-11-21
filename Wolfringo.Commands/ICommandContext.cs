using TehGM.Wolfringo.Messages;

namespace TehGM.Wolfringo.Commands
{
    /// <summary>Represents a context of command execution.</summary>
    public interface ICommandContext
    {
        /// <summary>Chat message that triggered the command.</summary>
        IChatMessage Message { get; }
        /// <summary>WOLF client that received the message.</summary>
        IWolfClient Client { get; }
        /// <summary>Default options to use for processing the command.</summary>
        ICommandsOptions Options { get; }
    }
}
