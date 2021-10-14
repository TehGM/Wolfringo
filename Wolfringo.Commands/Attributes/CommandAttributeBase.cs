using System;

namespace TehGM.Wolfringo.Commands.Attributes
{
    /// <summary>Marks a method as a Command for Commands System.</summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public abstract class CommandAttributeBase : Attribute
    {
        /// <summary>Timeout (in milliseconds) for commands execution. Defaults to 1 day.</summary>
        public int Timeout { get; set; } = (int)TimeSpan.FromDays(1).TotalMilliseconds;
    }
}
