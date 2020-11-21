using System;

namespace TehGM.Wolfringo.Commands
{
    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
    public class CommandHandlerConstructorAttribute : Attribute
    {
        public int Priority { get; }

        public CommandHandlerConstructorAttribute(int priority)
        {
            this.Priority = priority;
        }

        public CommandHandlerConstructorAttribute() { }
    }
}
