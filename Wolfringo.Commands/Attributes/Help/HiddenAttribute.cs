using System;

namespace TehGM.Wolfringo.Commands
{
    /// <summary>Hides commands or all commands in a handler from the help list.</summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class HiddenAttribute : Attribute { }
}
