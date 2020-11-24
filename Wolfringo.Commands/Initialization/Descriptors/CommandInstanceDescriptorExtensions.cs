using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TehGM.Wolfringo.Commands.Initialization
{
    /// <summary>Extensions for command instance descriptors.</summary>
    public static class CommandInstanceDescriptorExtensions
    {
        /// <summary>Retrieves CommandHandler attribute for the command's handler.</summary>
        /// <param name="descriptor">Command descriptor.</param>
        /// <returns>CommandHandler attribute present on the command's handler type; null if not found.</returns>
        public static CommandHandlerAttribute GetHandlerAttribute(this ICommandInstanceDescriptor descriptor)
            => GetHandlerType(descriptor).GetCustomAttribute<CommandHandlerAttribute>(true);

        /// <summary>Gets command's priority.</summary>
        /// <remarks>See <see cref="PriorityAttribute"/> for more information about command priorities.</remarks>
        /// <param name="descriptor">Command descriptor.</param>
        /// <returns>Command's priority value.</returns>
        /// <seealso cref="PriorityAttribute"/>
        public static int GetPriority(this ICommandInstanceDescriptor descriptor)
            // on-method priority overwrites handler priority. Default is 0.
            =>  descriptor.Method.GetCustomAttribute<PriorityAttribute>()?.Priority ??
                GetHandlerType(descriptor).GetCustomAttribute<PriorityAttribute>()?.Priority ??
                0;

        /// <summary>Gets type of the command's handler.</summary>
        /// <param name="descriptor">Command descriptor.</param>
        /// <returns>Type of command's handler.</returns>
        public static Type GetHandlerType(this ICommandInstanceDescriptor descriptor)
            => descriptor.Method.DeclaringType;

        /// <summary>Checks if the command is overriding default case sensitivity.</summary>
        /// <remarks>See <see cref="CaseSensitivityAttribute"/> for more information about command case senstivity.</remarks>
        /// <returns>True/false if command is overriding case sensitivity - true to be case sensitive, false to be case insensitive; null if not overriding.</returns>
        public static bool? GetCaseSensitivityOverride(this ICommandInstanceDescriptor descriptor)
            => descriptor.Method.GetCustomAttribute<CaseSensitivityAttribute>(true)?.CaseSensitive ??
                descriptor.GetHandlerType().GetCustomAttribute<CaseSensitivityAttribute>(true)?.CaseSensitive;

        /// <summary>Gets command's prefix override.</summary>
        /// <param name="descriptor">Command descriptor.</param>
        /// <returns>Prefix value.</returns>
        public static string GetPrefixOverride(this ICommandInstanceDescriptor descriptor)
            => descriptor.Method.GetCustomAttribute<PrefixAttribute>(true)?.PrefixOverride ??
                descriptor.GetHandlerType().GetCustomAttribute<PrefixAttribute>(true)?.PrefixOverride;

        /// <summary>Gets command's prefix requirement override.</summary>
        /// <param name="descriptor">Command descriptor.</param>
        /// <returns>Prefix requirement value.</returns>
        public static PrefixRequirement? GetPrefixRequirementOverride(this ICommandInstanceDescriptor descriptor)
            => descriptor.Method.GetCustomAttribute<PrefixAttribute>(true)?.PrefixRequirementOverride ??
                descriptor.GetHandlerType().GetCustomAttribute<PrefixAttribute>(true)?.PrefixRequirementOverride;

        /// <summary>Gets command's pre-execute checks.</summary>
        /// <param name="descriptor">Command descriptor.</param>
        /// <returns>Enumerable on all pre-execute checks on the command's method and handler type.</returns>
        public static IEnumerable<CommandRequirementAttribute> GetRequirements(this ICommandInstanceDescriptor descriptor)
            => descriptor.Method.GetCustomAttributes<CommandRequirementAttribute>(true)
                .Union(descriptor.GetHandlerType().GetCustomAttributes<CommandRequirementAttribute>(true));
    }
}
