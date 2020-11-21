using System;

namespace TehGM.Wolfringo.Commands.Initialization
{
    public interface ICommandHandlerDescriptor : IEquatable<ICommandHandlerDescriptor>
    {
        CommandHandlerAttribute Attribute { get; }
        Type Type { get; }
    }
}
