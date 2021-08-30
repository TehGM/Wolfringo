using System;

namespace TehGM.Wolfringo.Commands
{
    /// <summary>Sets display name for the command in the help list.</summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class DisplayNameAttribute : Attribute
    {
        /// <summary>Display Name value.</summary>
        public string Text { get; }

        /// <summary>Sets display name for the command in the help list.</summary>
        /// <param name="text">Display Name value.</param>
        public DisplayNameAttribute(string text)
        {
            this.Text = text;
        }
    }
}
