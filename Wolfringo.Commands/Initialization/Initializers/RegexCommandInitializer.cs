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

            // init instance
            return new RegexCommandInstance(
                pattern: regexCommand.Pattern,
                regexOptions: regexCommand.Options,
                method: descriptor.Method,
                requirements: descriptor.GetRequirements(),
                prefixOverride: descriptor.GetPrefixOverride(),
                prefixRequirementOverride: descriptor.GetPrefixRequirementOverride(),
                caseSensitivityOverride: descriptor.GetCaseSensitivityOverride());
        }
    }
}
