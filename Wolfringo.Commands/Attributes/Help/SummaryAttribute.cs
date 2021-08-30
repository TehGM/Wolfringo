using System;

namespace TehGM.Wolfringo.Commands
{
    /// <summary>Sets the summary description for the command in the help list.</summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class SummaryAttribute : Attribute, IEquatable<SummaryAttribute>
    {
        /// <summary>Summary value.</summary>
        public string Text { get; }

        /// <summary>Sets the summary description for the command in the help list.</summary>
        /// <param name="text">Summary value.</param>
        public SummaryAttribute(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentNullException(nameof(text), "Summary cannot be null, blank or whitespace");

            this.Text = text;
        }

        /// <inheritdoc/>
        public bool Equals(SummaryAttribute other)
            => other != null && this.Text.Equals(other.Text, StringComparison.Ordinal);

        /// <inheritdoc/>
        public override bool Equals(object obj)
            => this.Equals(obj as SummaryAttribute);

        /// <inheritdoc/>
        public override int GetHashCode()
            => this.Text.GetHashCode();
    }
}
