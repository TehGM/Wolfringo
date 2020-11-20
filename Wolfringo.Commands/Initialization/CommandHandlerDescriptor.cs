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

        public bool IsPersistent { get; }
        public bool IsPreInitialized { get; }

        public CommandHandlerDescriptor(ConstructorInfo ctor, IEnumerable<object> parameters)
        {
            this.Constructor = ctor;
            this.ConstructorParams = parameters.ToArray();

            // check [CommandHandler] attribute
            CommandHandlerAttribute commandHandlerAttribute = this.Type.GetCustomAttribute<CommandHandlerAttribute>();
            if (commandHandlerAttribute != null)
            {
                // shared handlers need to be persistent, so check both to true
                this.IsPersistent = commandHandlerAttribute.IsPersistent;
                this.IsPreInitialized = commandHandlerAttribute.PreInitialize;
            }
        }

        public object CreateInstance()
            => this.Constructor.Invoke(this.ConstructorParams);
    }
}
