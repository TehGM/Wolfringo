using System;

namespace TehGM.Wolfringo.Commands
{
    /// <inheritdoc/>
    /// <remarks>This attribute represents a normal, simple command.</remarks>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class CommandAttribute : CommandAttributeBase
    {
        /// <summary>Text that will trigger the command.</summary>
        /// <remarks>This text needs to be right after prefix, or at the beginning of the message if prefix is not required.</remarks>
        public string Text { get; }

        /// <param name="text">Text that will trigger the command.</param>
        public CommandAttribute(string text) : base()
        {
            this.Text = text;
        }
    }
}
