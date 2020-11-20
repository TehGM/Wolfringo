using TehGM.Wolfringo.Messages;

namespace TehGM.Wolfringo.Commands
{
    public interface ICommandContext
    {
        IChatMessage Message { get; }
        IWolfClient Client { get; }
        ICommandsOptions Options { get; }
    }
}
