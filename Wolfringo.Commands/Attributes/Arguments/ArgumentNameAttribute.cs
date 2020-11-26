using System;
using System.Collections.Generic;

namespace TehGM.Wolfringo.Commands
{
    /// <summary>Changes the name of attribute inside of error messages.</summary>
    /// <remarks>This name will be used by {{Name}} placeholder in error message template.</remarks>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class ArgumentNameAttribute : Attribute, IEquatable<ArgumentNameAttribute>
    {
        /// <summary>Display name for the argument.</summary>
        /// <remarks>This name will be used by {{Name}} placeholder in error message template.</remarks>
        public string Name { get; }

        /// <summary>Changes the name of attribute inside of error messages.</summary>
        /// <param name="name">Display name for the argument.</param>
        /// <remarks>This name will be used by {{Name}} placeholder in error message template.</remarks>
        public ArgumentNameAttribute(string name) : base()
        {
            this.Name = name;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return Equals(obj as ArgumentNameAttribute);
        }

        /// <inheritdoc/>
        public bool Equals(ArgumentNameAttribute other)
        {
            return other != null &&
                   Name == other.Name;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int hashCode = 890389916;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            return hashCode;
        }

        /// <inheritdoc/>
        public static bool operator ==(ArgumentNameAttribute left, ArgumentNameAttribute right)
        {
            return EqualityComparer<ArgumentNameAttribute>.Default.Equals(left, right);
        }

        /// <inheritdoc/>
        public static bool operator !=(ArgumentNameAttribute left, ArgumentNameAttribute right)
        {
            return !(left == right);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.Name;
        }
    }
}
