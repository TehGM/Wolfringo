using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TehGM.Wolfringo.Commands.Instances;

namespace TehGM.Wolfringo.Commands.Initialization
{
    /// <inheritdoc/>
    /// <remarks>This is a default initializer designed to load <see cref="RegexCommandInstance"/> from <see cref="RegexCommandAttribute"/>. It will not work with other command types.</remarks>
    public class RegexCommandInitializer : ICommandInitializer
    {
        /// <inheritdoc/>
        public ICommandInstance InitializeCommand(ICommandInstanceDescriptor descriptor, IServiceProvider services, ICommandsOptions options)
        {
            // validate this is a correct command attribute type
            if (!(descriptor.Attribute is RegexCommandAttribute regexCommand))
                throw new ArgumentException($"{this.GetType().Name} can only be used with {typeof(RegexCommandAttribute).Name} commands", nameof(descriptor.Attribute));

            // check case sensitiviness, Priority: method attribute, handler attribute, options
            bool caseInsensitive = descriptor.IsCaseInsensitive(defaultValue: options?.CaseInsensitive ?? false);

            // prepare regex
            RegexOptions regexOptions = regexCommand.Options;
            if (caseInsensitive)
                regexOptions |= RegexOptions.IgnoreCase;
            Regex regex = new Regex(regexCommand.Pattern, regexOptions);

            // read any additional requirements
            IEnumerable<CommandRequirementAttribute> requirements = descriptor.GetRequirements();

            // init instance
            return new RegexCommandInstance(regex, descriptor.Method, requirements);
        }
    }
}
