using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TehGM.Wolfringo.Commands.Initialization
{
    public class CommandHandlerDescriptor
    {
        public Type Type => this.Constructor.DeclaringType;
        public ConstructorInfo Constructor { get; }
        public object[] ConstructorParams { get; }

        public CommandHandlerAttribute Attribute { get; }

        public CommandHandlerDescriptor(ConstructorInfo ctor, IEnumerable<object> parameters)
        {
            this.Constructor = ctor;
            this.ConstructorParams = parameters.ToArray();

            // check [CommandHandler] attribute
            this.Attribute = this.Type.GetCustomAttribute<CommandHandlerAttribute>();
        }

        public object CreateInstance()
            => this.Constructor.Invoke(this.ConstructorParams);
    }
}
