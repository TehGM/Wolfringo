using System;
using System.Text.RegularExpressions;
using TehGM.Wolfringo.Commands.Attributes;

namespace TehGM.Wolfringo.Commands
{
    /// <inheritdoc/>
    /// <remarks>This attribute represents a regex command. It's designed to be initialized by <see cref="Initialization.RegexCommandInitializer"/>.</remarks>
    /// <seealso cref="Initialization.RegexCommandInitializer"/>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class RegexCommandAttribute : CommandAttributeBase
    {
        /// <summary>Indicates default timeout (in milliseconds) to use when parsing user's input. Equals 1000 milliseconds.</summary>
        public static int RegexDefaultTimeout { get; } = 1000;
        /// <summary>Default regex options for the regex command.</summary>
        public const RegexOptions DefaultOptions = RegexOptions.CultureInvariant | RegexOptions.Singleline;

        /// <summary>Regex pattern for the command.</summary>
        public string Pattern { get; }
        /// <summary>Regex options for the command.</summary>
        public RegexOptions Options { get; }
        /// <summary>Specifies timeout (in milliseconds) for Regex engine. <see cref="RegexDefaultTimeout"/> is used by default.</summary>
        public int RegexTimeout { get; set; } = RegexDefaultTimeout;

        /// <summary>Creates the attribute with specified regex settings.</summary>
        /// <param name="pattern">Regex pattern for the command.</param>
        /// <param name="options">Regex options for the command.</param>
        public RegexCommandAttribute(string pattern, RegexOptions options) : base()
        {
            if (pattern == null)
                throw new ArgumentNullException(nameof(pattern));
            this.Pattern = pattern;
            this.Options = options;
        }

        /// <summary>Creates the attribute, using default regex options.</summary>
        /// <param name="pattern">Regex pattern for the command.</param>
        /// <seealso cref="DefaultOptions"/>
        public RegexCommandAttribute(string pattern)
            : this(pattern, DefaultOptions) { }
    }
}
