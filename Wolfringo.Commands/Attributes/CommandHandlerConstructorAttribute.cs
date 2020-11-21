using System;

namespace TehGM.Wolfringo.Commands
{
    /// <summary>Marks handler's constructor as a constructor for Commands System.</summary>
    /// <remarks><para>By default, Commands System will only look at public constructors of the handler, on order depending on parameters count, from highest to lowest. This attribute allows for marking a specific constructor to be used. Marked constructor can be private.</para>
    /// <para>If multiple constructors are marked, <see cref="Priority"/> is used. Constructor with higher priority will always be tested first. If multiple constructors have the same priority, they're attempted in order depending on parameters count, from highest to lowest.</para></remarks>
    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = true, Inherited = false)]
    public class CommandHandlerConstructorAttribute : Attribute
    {
        /// <summary>Constructor's priority.</summary>
        /// <remarks>See <see cref="CommandHandlerConstructorAttribute"/> for more information about handler constructor priorities.</remarks>
        public int Priority { get; }

        /// <summary>Creates a new CommandHandlerConstructor attribute.</summary>
        /// <param name="priority">Constructor's priority.</param>
        /// <remarks>See <see cref="CommandHandlerConstructorAttribute"/> for more information about handler constructor priorities.</remarks>
        public CommandHandlerConstructorAttribute(int priority)
        {
            this.Priority = priority;
        }

        /// <summary>Creates a new CommandHandlerConstructor attribute.</summary>
        /// <remarks>See <see cref="CommandHandlerConstructorAttribute"/> for more information about handler constructor priorities.</remarks>
        public CommandHandlerConstructorAttribute() { }
    }
}
