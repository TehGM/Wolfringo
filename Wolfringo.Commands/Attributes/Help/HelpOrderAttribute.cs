using System;
using System.Collections.Generic;

namespace TehGM.Wolfringo.Commands
{
    /// <summary>Specifies command's help order.</summary>
    /// <remarks><para>Commands with higher order value will appear in help command earlier than commands with lower value.</para>
    /// <para>This attribute takes precedence over <see cref="PriorityAttribute"/> when constructing help command output.</para>
    /// <para>Command help order will be taken from <see cref="HelpOrderAttribute"/> present on the method. If the method doesn't have the attribute, it'll be taken from <see cref="HelpOrderAttribute"/> present on the handler type. If the handler type also doesn't specify the help order, <see cref="PriorityAttribute"/> will be checked. If <see cref="PriorityAttribute"/> is also not specified, default value of 0 will be used.</para></remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class HelpOrderAttribute : Attribute, IEquatable<HelpOrderAttribute>, IComparable<HelpOrderAttribute>
    {
        /// <summary>Order at which the command will be listed in help command.</summary>
        /// <remarks>See <see cref="HelpOrderAttribute"/> for more information about command help order.</remarks>
        public int Order { get; }

        /// <summary>Creates a new help order attribute.</summary>
        /// <param name="order">Command's help order.</param>
        /// <remarks>See <see cref="HelpOrderAttribute"/> for more information about command help order.</remarks>
        public HelpOrderAttribute(int order)
        {
            this.Order = order;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
            => this.Equals(obj as HelpOrderAttribute);

        /// <inheritdoc/>
        public bool Equals(HelpOrderAttribute other)
            => other != null && Order == other.Order;

        /// <inheritdoc/>
        public override int GetHashCode()
            => this.Order.GetHashCode();

        /// <inheritdoc/>
        public int CompareTo(HelpOrderAttribute other)
            => this.Order.CompareTo(other.Order);

        /// <inheritdoc/>
        public static bool operator ==(HelpOrderAttribute left, HelpOrderAttribute right)
            => EqualityComparer<HelpOrderAttribute>.Default.Equals(left, right);

        /// <inheritdoc/>
        public static bool operator !=(HelpOrderAttribute left, HelpOrderAttribute right)
            => !(left == right);
    }
}
