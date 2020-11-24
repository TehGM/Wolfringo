using System;

namespace TehGM.Wolfringo.Commands.Initialization
{
    /// <summary>An initializer that creates a command instance from a descriptor.</summary>
    public interface ICommandInitializer
    {
        /// <summary>Initializes a command instance.</summary>
        /// <param name="descriptor">Descriptor of the command.</param>
        /// <param name="handler">Handler object to init the command for.</param>
        /// <param name="options">Default options to initialize the instance with.</param>
        /// <returns>A new command instance.</returns>
        ICommandInstance InitializeCommand(ICommandInstanceDescriptor descriptor, IServiceProvider services, ICommandsOptions options);
    }
}
