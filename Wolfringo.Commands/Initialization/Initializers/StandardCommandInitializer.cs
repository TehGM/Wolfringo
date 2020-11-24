using System;
using System.Collections.Generic;
using System.Reflection;
using TehGM.Wolfringo.Commands.Instances;

namespace TehGM.Wolfringo.Commands.Initialization
{
    /// <inheritdoc/>
    /// <remarks>This is a default initializer designed to load <see cref="RegexCommandInstance"/> from <see cref="RegexCommandAttribute"/>. It will not work with other command types.</remarks>
    public class StandardCommandInitializer : ICommandInitializer
    {
        /// <inheritdoc/>
        public ICommandInstance InitializeCommand(ICommandInstanceDescriptor descriptor, ICommandsOptions options)
        {
            // validate this is a correct command attribute type
            if (!(descriptor.Attribute is CommandAttribute command))
                throw new ArgumentException($"{this.GetType().Name} can only be used with {typeof(CommandAttribute).Name} commands", nameof(descriptor.Attribute));

            // check case sensitiviness, Priority: method attribute, handler attribute, options
            bool? caseInsensitive = descriptor.Method.GetCustomAttribute<CaseInsensitiveAttribute>(true)?.CaseInsensitive ??
                descriptor.GetHandlerType().GetCustomAttribute<CaseInsensitiveAttribute>(true)?.CaseInsensitive;

            // read any additional requirements
            IEnumerable<CommandRequirementAttribute> requirements = descriptor.GetRequirements();

            // init instance
            return new StandardCommandInstance(command.Text, caseInsensitive, descriptor.Method, requirements);
        }
    }
}
