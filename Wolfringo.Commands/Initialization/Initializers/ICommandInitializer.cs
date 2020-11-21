namespace TehGM.Wolfringo.Commands.Initialization
{
    public interface ICommandInitializer
    {
        ICommandInstance InitializeCommand(ICommandInstanceDescriptor descriptor, object handler, ICommandsOptions options);
    }
}
