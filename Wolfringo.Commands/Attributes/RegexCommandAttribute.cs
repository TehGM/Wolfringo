using System;
using System.Text.RegularExpressions;

namespace TehGM.Wolfringo.Commands
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class RegexCommandAttribute : CommandAttributeBase
    {
        public const RegexOptions DefaultOptions = RegexOptions.CultureInvariant | RegexOptions.Multiline | RegexOptions.Compiled;

        public string Pattern { get; }
        public RegexOptions Options { get; }

        public RegexCommandAttribute(string pattern, RegexOptions options) : base()
        {
            this.Pattern = pattern;
            this.Options = options;
        }

        public RegexCommandAttribute(string pattern)
            : this(pattern, DefaultOptions) { }
    }
}
