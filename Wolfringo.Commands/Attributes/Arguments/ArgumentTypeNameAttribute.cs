using System;
using System.Collections.Generic;

namespace TehGM.Wolfringo.Commands
{
    /// <summary>Changes the type of attribute inside of error messages.</summary>
    /// <remarks>This name will be used by {{Type}} placeholder in error message template.</remarks>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class ArgumentTypeNameAttribute : Attribute, IEquatable<ArgumentTypeNameAttribute>
    {
        /// <summary>Changes the type of attribute inside of error messages.</summary>
        /// <remarks>This name will be used by {{Type}} placeholder in error message template.</remarks>
        public string Name { get; }

        /// <summary>Changes the type of attribute inside of error messages.</summary>
        /// <param name="name">Display name for the argument type.</param>
        /// <remarks>This name will be used by {{Type}} placeholder in error message template.</remarks>
        public ArgumentTypeNameAttribute(string name) : base()
        {
            this.Name = name;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return Equals(obj as ArgumentTypeNameAttribute);
        }

        /// <inheritdoc/>
        public bool Equals(ArgumentTypeNameAttribute other)
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
        public static bool operator ==(ArgumentTypeNameAttribute left, ArgumentTypeNameAttribute right)
        {
            return EqualityComparer<ArgumentTypeNameAttribute>.Default.Equals(left, right);
        }

        /// <inheritdoc/>
        public static bool operator !=(ArgumentTypeNameAttribute left, ArgumentTypeNameAttribute right)
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
