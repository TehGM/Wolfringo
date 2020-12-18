namespace TehGM.Wolfringo.Commands.Initialization
{
    /// <inheritdoc/>
    public class CommandsHandlerProviderResult : ICommandsHandlerProviderResult
    {
        /// <inheritdoc/>
        public ICommandHandlerDescriptor Descriptor { get; }

        /// <inheritdoc/>
        public object HandlerInstance { get; }

        /// <summary>Creates a new handler provider result.</summary>
        /// <param name="descriptor">Descriptor of the handler.</param>
        /// <param name="handler">Initialized instance of the handler.</param>
        public CommandsHandlerProviderResult(ICommandHandlerDescriptor descriptor, object handler)
        {
            this.Descriptor = descriptor;
            this.HandlerInstance = handler;
        }
    }
}
