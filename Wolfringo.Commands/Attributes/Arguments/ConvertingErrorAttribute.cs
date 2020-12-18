using System;
using TehGM.Wolfringo.Commands.Attributes;

namespace TehGM.Wolfringo.Commands
{
    /// <summary>Attribute to specify converting error message that will be sent as resposne on error.</summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class ConvertingErrorAttribute : ArgumentErrorAttribute
    {
        /// <summary>Default instance containing a default message template.</summary>
        public static ConvertingErrorAttribute Default { get; set; } = new ConvertingErrorAttribute("(n) '{{Arg}}' is not a valid {{Type}}");

        /// <inheritdoc/>
        public ConvertingErrorAttribute(string messageTemplate) : base(messageTemplate) { }
    }
}
