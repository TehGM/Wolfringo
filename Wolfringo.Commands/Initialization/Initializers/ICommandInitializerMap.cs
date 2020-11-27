using System;

namespace TehGM.Wolfringo.Commands.Initialization
{
    /// <summary>A map of command attributes, and the initializers that should be used with them.</summary>
    public interface ICommandInitializerMap
    {
        /// <summary>Gets a command initializer for the command type.</summary>
        /// <param name="commandAttributeType">Type of the command.</param>
        /// <returns>An initializer for provided command type.</returns>
        ICommandInitializer GetInitializer(Type commandAttributeType);
    }
}
