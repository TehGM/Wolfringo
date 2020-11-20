using System;
using System.Text.RegularExpressions;
using TehGM.Wolfringo.Commands.Instances;

namespace TehGM.Wolfringo.Commands.Initialization
{
    public class RegexCommandInitializer : ICommandInitializer
    {
        public ICommandInstance InitializeCommand(ICommandInstanceDescriptor descriptor, object handler)
        {
            // validate this is a correct command attribute type
            if (!(descriptor.Attribute is RegexCommandAttribute regexCommand))
                throw new ArgumentException($"{this.GetType().Name} can only be used with {typeof(RegexCommandAttribute).Name} commands", nameof(descriptor.Attribute));

            // prepare regex
            RegexOptions options = regexCommand.Options;
            if (regexCommand.OverrideCaseInsensitive == null || regexCommand.OverrideCaseInsensitive == true)
                options |= RegexOptions.IgnoreCase;
            Regex regex = new Regex(regexCommand.Pattern, options);

            // init instance
            return new RegexCommandInstance(regex, descriptor.Method, handler);
        }
    }
}
