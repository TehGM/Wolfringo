using System;
using TehGM.Wolfringo.Commands.Instances;

namespace TehGM.Wolfringo.Commands.Initialization
{
    /// <inheritdoc/>
    /// <remarks>This is a default initializer designed to load <see cref="RegexCommandInstance"/> from <see cref="RegexCommandAttribute"/>. It will not work with other command types.</remarks>
    public class RegexCommandInitializer : ICommandInitializer
    {
        /// <inheritdoc/>
        public ICommandInstance InitializeCommand(ICommandInstanceDescriptor descriptor, ICommandsOptions options)
        {
            // validate this is a correct command attribute type
            if (!(descriptor.Attribute is RegexCommandAttribute regexCommand))
                throw new ArgumentException($"{this.GetType().Name} can only be used with {typeof(RegexCommandAttribute).Name} commands", nameof(descriptor.Attribute));

            // if pattern starts with ^, replace it with \G
            // this will ensure it'll match start of the string when starting from index after prefix
            string pattern = regexCommand.Pattern;
            if (pattern.Length > 0 && pattern[0] == '^')
                pattern = $@"\G{pattern.Substring(1)}";

            // init instance
            return new RegexCommandInstance(
                pattern: pattern,
                regexOptions: regexCommand.Options,
                method: descriptor.Method,
                requirements: descriptor.GetRequirements(),
                prefixOverride: descriptor.GetPrefixOverride(),
                prefixRequirementOverride: descriptor.GetPrefixRequirementOverride(),
                caseSensitivityOverride: descriptor.GetCaseSensitivityOverride());
        }
    }
}
