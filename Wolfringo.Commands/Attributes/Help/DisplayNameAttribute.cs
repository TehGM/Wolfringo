using System;

namespace TehGM.Wolfringo.Commands
{
    /// <summary>Sets display name for the command in the help list.</summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class DisplayNameAttribute : Attribute, IEquatable<DisplayNameAttribute>
    {
        /// <summary>Display Name value.</summary>
        public string Text { get; }

        /// <summary>Sets display name for the command in the help list.</summary>
        /// <param name="text">Display Name value.</param>
        public DisplayNameAttribute(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentNullException(nameof(text), "Display name cannot be null, blank or whitespace");

            this.Text = text;
        }

        /// <inheritdoc/>
        public bool Equals(DisplayNameAttribute other)
            => other != null && this.Text.Equals(other.Text, StringComparison.Ordinal);

        /// <inheritdoc/>
        public override bool Equals(object obj)
            => this.Equals(obj as DisplayNameAttribute);

        /// <inheritdoc/>
        public override int GetHashCode()
            => this.Text.GetHashCode();
    }
}
