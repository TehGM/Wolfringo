using System;
using System.Collections.Generic;

namespace TehGM.Wolfringo.Commands
{
    /// <summary>Specifies command's priority.</summary>
    /// <remarks><para>Commands with higher priority value will run before commands with lower value.</para>
    /// <para>Depending on on the command service used, only one command can run per message. In such case, out of matching commands, only the one with highest priority value will run.<br/>
    /// This is the behaviour of command services included with Wolfringo library by default.</para>
    /// <para>Command priority will be taken from <see cref="PriorityAttribute"/> present on the method. If the method doesn't have the attribute, it'll be taken from <see cref="PriorityAttribute"/> present on the handler type. If the handler type also doesn't specify the priority, default value of 0 will be used.</para></remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class PriorityAttribute : Attribute, IEquatable<PriorityAttribute>, IComparable<PriorityAttribute>
    {
        /// <summary>Command's priority.</summary>
        /// <remarks>See <see cref="PriorityAttribute"/> for more information about command priorities.</remarks>
        public int Priority { get; }

        /// <summary>Creates a new priority attribute.</summary>
        /// <param name="priority">Command's priority.</param>
        /// <remarks>See <see cref="PriorityAttribute"/> for more information about command priorities.</remarks>
        public PriorityAttribute(int priority)
        {
            this.Priority = priority;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
            => this.Equals(obj as PriorityAttribute);

        /// <inheritdoc/>
        public bool Equals(PriorityAttribute other)
            => other != null && Priority == other.Priority;

        /// <inheritdoc/>
        public override int GetHashCode()
            => this.Priority.GetHashCode();

        /// <inheritdoc/>
        public int CompareTo(PriorityAttribute other)
            => this.Priority.CompareTo(other.Priority);

        /// <inheritdoc/>
        public static bool operator ==(PriorityAttribute left, PriorityAttribute right)
            => EqualityComparer<PriorityAttribute>.Default.Equals(left, right);

        /// <inheritdoc/>
        public static bool operator !=(PriorityAttribute left, PriorityAttribute right)
            => !(left == right);
    }
}
