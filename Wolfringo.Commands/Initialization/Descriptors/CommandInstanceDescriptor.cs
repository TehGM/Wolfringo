using System.Collections.Generic;
using System.Reflection;
using TehGM.Wolfringo.Commands.Attributes;

namespace TehGM.Wolfringo.Commands.Initialization
{
    /// <inheritdoc/>
    public class CommandInstanceDescriptor : ICommandInstanceDescriptor
    {
        /// <inheritdoc/>
        public CommandAttributeBase Attribute { get; }
        /// <inheritdoc/>
        public MethodInfo Method { get; }
        /// <summary>Command Handler Attribute present on the handler. Might be null if handler isn't tagged with that attribute.</summary>
        public CommandsHandlerAttribute HandlerAttribute { get; }

        /// <summary>Creates a command descriptor.</summary>
        /// <param name="attribute">Command attribute that specifies this command.</param>
        /// <param name="method">Method that will be run when the command executes.</param>
        public CommandInstanceDescriptor(CommandAttributeBase attribute, MethodInfo method)
        {
            this.Attribute = attribute;
            this.Method = method;

            // from extensions
            this.HandlerAttribute = this.GetHandlerAttribute();
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return Equals(obj as CommandInstanceDescriptor);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int hashCode = 2045227319;
            hashCode = hashCode * -1521134295 + EqualityComparer<CommandAttributeBase>.Default.GetHashCode(Attribute);
            hashCode = hashCode * -1521134295 + EqualityComparer<MethodInfo>.Default.GetHashCode(Method);
            return hashCode;
        }

        /// <inheritdoc/>
        public bool Equals(ICommandInstanceDescriptor other)
        {
            return other != null &&
                   EqualityComparer<CommandAttributeBase>.Default.Equals(Attribute, other.Attribute) &&
                   EqualityComparer<MethodInfo>.Default.Equals(Method, other.Method);
        }

        /// <inheritdoc/>
        public static bool operator ==(CommandInstanceDescriptor left, CommandInstanceDescriptor right)
        {
            return EqualityComparer<CommandInstanceDescriptor>.Default.Equals(left, right);
        }

        /// <inheritdoc/>
        public static bool operator !=(CommandInstanceDescriptor left, CommandInstanceDescriptor right)
        {
            return !(left == right);
        }
    }
}
