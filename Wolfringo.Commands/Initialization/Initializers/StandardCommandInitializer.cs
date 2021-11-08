using System;

namespace TehGM.Wolfringo.Commands.Initialization
{
    /// <inheritdoc/>
    /// <remarks>This is a default initializer designed to load <see cref="RegexCommandInstance"/> from <see cref="RegexCommandAttribute"/>. It will not work with other command types.</remarks>
    public class StandardCommandInitializer : ICommandInitializer
    {
        /// <inheritdoc/>
        public ICommandInstance InitializeCommand(ICommandInstanceDescriptor descriptor, CommandsOptions options)
        {
            // validate this is a correct command attribute type
            if (!(descriptor.Attribute is CommandAttribute command))
                throw new ArgumentException($"{this.GetType().Name} can only be used with {typeof(CommandAttribute).Name} commands", nameof(descriptor.Attribute));

            // init instance
            return new StandardCommandInstance(
                text: command.Text,
                method: descriptor.Method,
                requirements: descriptor.GetRequirements(),
                prefixOverride: descriptor.GetPrefixOverride(),
                prefixRequirementOverride: descriptor.GetPrefixRequirementOverride(),
                caseSensitivityOverride: descriptor.GetCaseSensitivityOverride(),
                timeout: command.Timeout);
        }
    }
}
