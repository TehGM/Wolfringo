using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TehGM.Wolfringo.Commands.Initialization
{
    public class CommandHandlerDescriptor : ICommandHandlerDescriptor
    {
        public ConstructorInfo Constructor { get; }
        public object[] ConstructorParams { get; }

        public CommandHandlerAttribute Attribute { get; }

        public Type Type => this.Constructor.DeclaringType;

        public CommandHandlerDescriptor(ConstructorInfo ctor, IEnumerable<object> parameters)
        {
            this.Constructor = ctor;
            this.ConstructorParams = parameters.ToArray();

            // check [CommandHandler] attribute
            this.Attribute = this.Constructor.DeclaringType.GetCustomAttribute<CommandHandlerAttribute>();
        }

        public object CreateInstance()
            => this.Constructor.Invoke(this.ConstructorParams);

        public override bool Equals(object obj)
        {
            return Equals(obj as CommandHandlerDescriptor);
        }

        public bool Equals(ICommandHandlerDescriptor other)
        {
            return other != null &&
                   EqualityComparer<Type>.Default.Equals(Type, other.Type) &&
                   EqualityComparer<CommandHandlerAttribute>.Default.Equals(Attribute, other.Attribute);
        }

        public override int GetHashCode()
        {
            int hashCode = -849586986;
            hashCode = hashCode * -1521134295 + EqualityComparer<ConstructorInfo>.Default.GetHashCode(Constructor);
            hashCode = hashCode * -1521134295 + EqualityComparer<CommandHandlerAttribute>.Default.GetHashCode(Attribute);
            return hashCode;
        }

        public static bool operator ==(CommandHandlerDescriptor left, CommandHandlerDescriptor right)
        {
            return EqualityComparer<CommandHandlerDescriptor>.Default.Equals(left, right);
        }

        public static bool operator !=(CommandHandlerDescriptor left, CommandHandlerDescriptor right)
        {
            return !(left == right);
        }
    }
}
