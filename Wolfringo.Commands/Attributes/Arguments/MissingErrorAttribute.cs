using TehGM.Wolfringo.Commands.Attributes;

namespace TehGM.Wolfringo.Commands
{
    /// <summary>Attribute to specify converting error message that will be sent as resposne on error.</summary>
    public class MissingErrorAttribute : ArgumentErrorAttribute
    {
        /// <summary>Default instance containing a default message template.</summary>
        public static MissingErrorAttribute Default { get; set; } = new MissingErrorAttribute("(n) Please provide {{Name}} argument!");

        /// <inheritdoc/>
        public MissingErrorAttribute(string messageTemplate) : base(messageTemplate) { }
    }
}
