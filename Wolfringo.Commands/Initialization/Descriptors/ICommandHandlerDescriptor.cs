using System;

namespace TehGM.Wolfringo.Commands.Initialization
{
    /// <summary>Describes a command handler.</summary>
    public interface ICommandHandlerDescriptor : IEquatable<ICommandHandlerDescriptor>
    {
        /// <summary>CommandHandler attribute the handler is decorated with.</summary>
        /// <remarks>This value might be null if descriptor was force-loaded from a type that wasn't automatically discovered.</remarks>
        CommandHandlerAttribute Attribute { get; }
        /// <summary>Command handler type.</summary>
        Type Type { get; }
    }
}
