using System;

namespace TehGM.Wolfringo.Commands
{
    /// <summary>Assigns command or all commands in the handler to a specific category in help list.</summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class HelpCategoryAttribute : Attribute, IEquatable<HelpCategoryAttribute>
    {
        /// <summary>Name of the category.</summary>
        public string Name { get; }
        /// <summary>Priority of the category.</summary>
        public int Priority { get; set; }

        private readonly Lazy<int> _hashcode;

        /// <summary>Assigns command or all commands in the handler to a specific category in help list.</summary>
        /// <param name="name">Name of the category.</param>
        /// <param name="priority">Priority of the category.</param>
        public HelpCategoryAttribute(string name, int priority)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name), "Category name cannot be null, blank or whitespace");

            this.Name = name.Trim();
            this.Priority = priority;
            this._hashcode = new Lazy<int>(() => this.Name.ToLowerInvariant().GetHashCode());
        }

        /// <summary>Assigns command or all commands in the handler to a specific category in help list, with priority of 0.</summary>
        /// <param name="name">Name of the category.</param>
        public HelpCategoryAttribute(string name)
            : this(name, 0) { }

        /// <summary>Check if the categories should be treated as the same.</summary>
        /// <remarks>Categories are treated the same if the name matches, case insensitively.</remarks>
        /// <param name="other">The category attribute to compare with.</param>
        /// <returns>True if the categories are considered to be the same; otherwise false.</returns>
        public bool Equals(HelpCategoryAttribute other)
            => other != null && this.Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase);

        /// <summary>Check if the categories should be treated as the same.</summary>
        /// <remarks>Categories are treated the same if the name matches, case insensitively.</remarks>
        /// <param name="obj">The category attribute to compare with.</param>
        /// <returns>True if <paramref name="obj"/> is <see cref="HelpCategoryAttribute"/> and the categories are considered to be the same; otherwise false.</returns>
        public override bool Equals(object obj)
            => this.Equals(obj as HelpCategoryAttribute);

        /// <inheritdoc/>
        public override int GetHashCode()
            => this._hashcode.Value;
    }
}
