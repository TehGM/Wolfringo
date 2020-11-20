using System;

namespace TehGM.Wolfringo.Commands
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public abstract class CommandAttributeBase : Attribute
    {
    }
}
