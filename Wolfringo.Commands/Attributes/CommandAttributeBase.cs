using System;

namespace TehGM.Wolfringo.Commands.Attributes
{
    /// <summary>Marks a method as a Command for Commands System.</summary>
    /// <remarks>This attribute is abstract and serves as nothing more than a base for actual command attributes.</remarks>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public abstract class CommandAttributeBase : Attribute { }
}
