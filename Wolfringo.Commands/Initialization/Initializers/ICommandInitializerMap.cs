using System;

namespace TehGM.Wolfringo.Commands.Initialization
{
    /// A map of command attributes, and the initializers that should be used with them.
    public interface ICommandInitializerMap
    {
        /// <summary>Gets a command initializer for the command type.</summary>
        /// <param name="commandAttributeType">Type of the command.</param>
        /// <returns>An initializer for provided command type.</returns>
        ICommandInitializer GetMappedInitializer(Type commandAttributeType);
        /// <summary>Maps a command type to an initializer.</summary>
        /// <param name="commandAttributeType">Type of the command.</param>
        /// <param name="initializer">Initializer to use for that command type.</param>
        void MapInitializer(Type commandAttributeType, ICommandInitializer initializer);
    }
}
