using System;
using System.Collections.Generic;

namespace TehGM.Wolfringo.Commands
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class PriorityAttribute : Attribute, IEquatable<PriorityAttribute>, IComparable<PriorityAttribute>
    {
        public int Priority { get; }

        public PriorityAttribute(int priority)
        {
            this.Priority = priority;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as PriorityAttribute);
        }

        public bool Equals(PriorityAttribute other)
        {
            return other != null &&
                   base.Equals(other) &&
                   Priority == other.Priority;
        }

        public override int GetHashCode()
        {
            int hashCode = 197095871;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + Priority.GetHashCode();
            return hashCode;
        }

        public int CompareTo(PriorityAttribute other)
            => this.Priority.CompareTo(other.Priority);

        public static bool operator ==(PriorityAttribute left, PriorityAttribute right)
        {
            return EqualityComparer<PriorityAttribute>.Default.Equals(left, right);
        }

        public static bool operator !=(PriorityAttribute left, PriorityAttribute right)
        {
            return !(left == right);
        }
    }
}
