using System;

namespace TehGM.Wolfringo.Commands.Initialization
{
    /// <summary>A provider of command initializers for specific command type.</summary>
    public interface ICommandInitializerProvider
    {
        /// <summary>Gets a command initializer for the command type.</summary>
        /// <param name="commandAttributeType">Type of the command.</param>
        /// <returns>An initializer for provided command type.</returns>
        ICommandInitializer GetInitializer(Type commandAttributeType);
    }
}
