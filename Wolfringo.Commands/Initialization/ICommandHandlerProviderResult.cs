namespace TehGM.Wolfringo.Commands.Initialization
{
    /// <summary>Represents a handler search result for handler provider.</summary>
    public interface ICommandHandlerProviderResult
    {
        /// <summary>Descriptor of the handler.</summary>
        ICommandHandlerDescriptor Descriptor { get; }
        /// <summary>Initialized instance of the handler.</summary>
        object HandlerInstance { get; }
    }
}
