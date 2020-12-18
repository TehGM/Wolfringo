using System;
using System.Reflection;
using TehGM.Wolfringo.Commands.Attributes;

namespace TehGM.Wolfringo.Commands.Initialization
{
    /// <summary>Describes a command instance.</summary>
    public interface ICommandInstanceDescriptor : IEquatable<ICommandInstanceDescriptor>
    {
        /// <summary>Command attribute that specifies this command.</summary>
        CommandAttributeBase Attribute { get; }
        /// <remarks>Method that will be run when the command executes.</remarks>
        MethodInfo Method { get; }
    }
}
