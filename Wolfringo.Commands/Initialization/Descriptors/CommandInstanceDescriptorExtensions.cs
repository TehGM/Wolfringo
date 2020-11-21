using System;
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

        /// <summary>Checks if the command is case insensitive.</summary>
        /// <param name="descriptor">Command descriptor.</param>
        /// <param name="defaultValue">Fallback value to use in case of <see cref="CaseInsensitiveAttribute"/> not being set on command method or handler. This is likely a value specified by <see cref="ICommandsOptions"/>.</param>
        /// <remarks>See <see cref="CaseInsensitiveAttribute"/> for more information about command case insensitiviness.</remarks>
        /// <returns>True if the command is case insensitive; otherwise false.</returns>
        public static bool IsCaseInsensitive(this ICommandInstanceDescriptor descriptor, bool defaultValue)
            => descriptor.Method.GetCustomAttribute<CaseInsensitiveAttribute>(true)?.CaseInsensitive ??
                descriptor.Method.DeclaringType.GetCustomAttribute<CaseInsensitiveAttribute>(true)?.CaseInsensitive ??
                defaultValue;
    }
}
