using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TehGM.Wolfringo.Commands.Initialization
{
    /// <inheritdoc/>
    public class CommandHandlerDescriptor : ICommandHandlerDescriptor
    {
        /// <summary>Constructor picked for creating the handler instance.</summary>
        public ConstructorInfo Constructor { get; }
        /// <summary>Dependencies resolved for constructor injection.</summary>
        public object[] ConstructorParams { get; }
        /// <inheritdoc/>
        public CommandHandlerAttribute Attribute { get; }
        /// <inheritdoc/>
        public Type Type => this.Constructor.DeclaringType;

        /// <summary>creates a command handler descriptor.</summary>
        /// <param name="ctor">Constructor picked for creating the handler instance.</param>
        /// <param name="parameters">Dependencies resolved for constructor injection.</param>
        public CommandHandlerDescriptor(ConstructorInfo ctor, IEnumerable<object> parameters)
        {
            this.Constructor = ctor;
            this.ConstructorParams = parameters.ToArray();

            // check [CommandHandler] attribute
            this.Attribute = this.Constructor.DeclaringType.GetCustomAttribute<CommandHandlerAttribute>();
        }

        /// <summary>Creates a new instance of a handler.</summary>
        /// <remarks>This method will call <see cref="Constructor"/>, injecting all <see cref="ConstructorParams"/>.</remarks>
        /// <returns>New handler instance.</returns>
        public object CreateInstance()
            => this.Constructor.Invoke(this.ConstructorParams);

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return Equals(obj as CommandHandlerDescriptor);
        }

        /// <inheritdoc/>
        public bool Equals(ICommandHandlerDescriptor other)
        {
            return other != null &&
                   EqualityComparer<Type>.Default.Equals(Type, other.Type) &&
                   EqualityComparer<CommandHandlerAttribute>.Default.Equals(Attribute, other.Attribute);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int hashCode = -849586986;
            hashCode = hashCode * -1521134295 + EqualityComparer<ConstructorInfo>.Default.GetHashCode(Constructor);
            hashCode = hashCode * -1521134295 + EqualityComparer<CommandHandlerAttribute>.Default.GetHashCode(Attribute);
            return hashCode;
        }

        /// <inheritdoc/>
        public static bool operator ==(CommandHandlerDescriptor left, CommandHandlerDescriptor right)
        {
            return EqualityComparer<CommandHandlerDescriptor>.Default.Equals(left, right);
        }

        /// <inheritdoc/>
        public static bool operator !=(CommandHandlerDescriptor left, CommandHandlerDescriptor right)
        {
            return !(left == right);
        }
    }
}
