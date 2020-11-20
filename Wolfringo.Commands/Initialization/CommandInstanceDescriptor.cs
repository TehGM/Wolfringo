using System;
using System.Collections.Generic;
using System.Reflection;

namespace TehGM.Wolfringo.Commands.Initialization
{
    public class CommandInstanceDescriptor : ICommandInstanceDescriptor
    {
        public CommandAttributeBase Attribute { get; }
        public MethodInfo Method { get; }
        public CommandHandlerAttribute HandlerAttribute { get; }
        public int Priority { get; }

        public Type HandlerType => Method.DeclaringType;

        public CommandInstanceDescriptor(CommandAttributeBase attribute, MethodInfo method)
        {
            this.Attribute = attribute;
            this.Method = method;

            this.HandlerAttribute = method.DeclaringType.GetCustomAttribute<CommandHandlerAttribute>(true);

            // on-method priority overwrites handler priority. Default is 0.
            this.Priority =
                method.GetCustomAttribute<PriorityAttribute>()?.Priority ??
                method.DeclaringType.GetCustomAttribute<PriorityAttribute>()?.Priority ??
                0;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as CommandInstanceDescriptor);
        }

        public override int GetHashCode()
        {
            int hashCode = 2045227319;
            hashCode = hashCode * -1521134295 + EqualityComparer<CommandAttributeBase>.Default.GetHashCode(Attribute);
            hashCode = hashCode * -1521134295 + EqualityComparer<MethodInfo>.Default.GetHashCode(Method);
            return hashCode;
        }

        public bool Equals(ICommandInstanceDescriptor other)
        {
            return other != null &&
                   EqualityComparer<CommandAttributeBase>.Default.Equals(Attribute, other.Attribute) &&
                   EqualityComparer<MethodInfo>.Default.Equals(Method, other.Method);
        }

        public static bool operator ==(CommandInstanceDescriptor left, CommandInstanceDescriptor right)
        {
            return EqualityComparer<CommandInstanceDescriptor>.Default.Equals(left, right);
        }

        public static bool operator !=(CommandInstanceDescriptor left, CommandInstanceDescriptor right)
        {
            return !(left == right);
        }
    }

    public interface ICommandInstanceDescriptor : IEquatable<ICommandInstanceDescriptor>
    {
        CommandAttributeBase Attribute { get; }
        MethodInfo Method { get; }
    }
}
