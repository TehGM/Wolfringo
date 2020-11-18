using System;

namespace TehGM.Wolfringo.Commands
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class CommandAttribute : CommandAttributeBase
    {
        /// <summary>Text that will trigger the command.</summary>
        /// <remarks>This text needs to be right after prefix, or at the beginning of the message if prefix is not required.</remarks>
        public string Text { get; }

        public CommandAttribute(string text) : base()
        {
            this.Text = text;
        }
    }
}
