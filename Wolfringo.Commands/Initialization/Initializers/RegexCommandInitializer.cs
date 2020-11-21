using System;
using System.Reflection;
using System.Text.RegularExpressions;
using TehGM.Wolfringo.Commands.Instances;

namespace TehGM.Wolfringo.Commands.Initialization
{
    public class RegexCommandInitializer : ICommandInitializer
    {
        public ICommandInstance InitializeCommand(ICommandInstanceDescriptor descriptor, object handler, ICommandsOptions options)
        {
            // validate this is a correct command attribute type
            if (!(descriptor.Attribute is RegexCommandAttribute regexCommand))
                throw new ArgumentException($"{this.GetType().Name} can only be used with {typeof(RegexCommandAttribute).Name} commands", nameof(descriptor.Attribute));

            // check case sensitiviness, Priority: method attribute, handler attribute, options
            bool caseInsensitive =
                descriptor.Method.GetCustomAttribute<CaseInsensitiveAttribute>(true)?.CaseInsensitive ??
                descriptor.Method.DeclaringType.GetCustomAttribute<CaseInsensitiveAttribute>(true)?.CaseInsensitive ??
                options?.CaseInsensitive ?? false;

            // prepare regex
            RegexOptions regexOptions = regexCommand.Options;
            if (caseInsensitive)
                regexOptions |= RegexOptions.IgnoreCase;
            Regex regex = new Regex(regexCommand.Pattern, regexOptions);

            // init instance
            return new RegexCommandInstance(regex, descriptor.Method, handler);
        }
    }
}
