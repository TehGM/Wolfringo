namespace TehGM.Wolfringo.Commands.Initialization
{
    public interface ICommandHandlerProvider
    {
        object GetCommandHandler(ICommandInstanceDescriptor descriptor);
    }
}
