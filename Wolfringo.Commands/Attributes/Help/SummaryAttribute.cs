using System;

namespace TehGM.Wolfringo.Commands
{
    /// <summary>Sets the summary description for the command in the help list.</summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class SummaryAttribute : Attribute
    {
        /// <summary>Summary value.</summary>
        public string Text { get; }

        /// <summary>Sets the summary description for the command in the help list.</summary>
        /// <param name="text">Summary value.</param>
        public SummaryAttribute(string text)
        {
            this.Text = text;
        }
    }
}
