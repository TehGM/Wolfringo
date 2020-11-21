namespace TehGM.Wolfringo.Commands.Initialization
{
    /// <summary>A service that deals with resolving Handler instance to run command in.</summary>
    public interface ICommandHandlerProvider
    {
        /// <summary>Resolves a command handler instance.</summary>
        /// <param name="descriptor">Descriptor of the command instance.</param>
        /// <returns>An instance of a command handler.</returns>
        object GetCommandHandler(ICommandInstanceDescriptor descriptor);
    }
}
