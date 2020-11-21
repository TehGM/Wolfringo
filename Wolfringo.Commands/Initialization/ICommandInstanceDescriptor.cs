using System;
using System.Reflection;

namespace TehGM.Wolfringo.Commands.Initialization
{
    public interface ICommandInstanceDescriptor : IEquatable<ICommandInstanceDescriptor>
    {
        CommandAttributeBase Attribute { get; }
        MethodInfo Method { get; }
    }
}
