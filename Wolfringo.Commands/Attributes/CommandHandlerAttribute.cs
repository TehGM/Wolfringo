using System;

namespace TehGM.Wolfringo.Commands
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class CommandHandlerAttribute : Attribute
    {
        public bool IsPersistent { get; set; } = false;
        public bool PreInitialize { get; set; } = false;
    }
}
