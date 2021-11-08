using System;
using System.Collections.Generic;
using System.Reflection;
using TehGM.Wolfringo.Commands.Attributes;
using TehGM.Wolfringo.Commands.Initialization;

namespace TehGM.Wolfringo.Commands
{
    /// <summary>Extensions for command instance descriptors.</summary>
    /// <remarks>Extensions in this class will automatically cache results to improve performance.</remarks>
    public static class CommandInstanceDescriptorExtensions
    {
        /// <summary>Retrieves CommandHandler attribute for the command's handler.</summary>
        /// <param name="descriptor">Command descriptor.</param>
        /// <returns>CommandHandler attribute present on the command's handler type; null if not found.</returns>
        public static CommandsHandlerAttribute GetHandlerAttribute(this ICommandInstanceDescriptor descriptor)
            => GetHandlerType(descriptor).GetCustomAttribute<CommandsHandlerAttribute>(true);

        /// <summary>Gets command's priority.</summary>
        /// <remarks>See <see cref="PriorityAttribute"/> for more information about command priorities.</remarks>
        /// <param name="descriptor">Command descriptor.</param>
        /// <returns>Command's priority value.</returns>
        /// <seealso cref="PriorityAttribute"/>
        public static int GetPriority(this ICommandInstanceDescriptor descriptor)
            => DescriptorAttributeCache.GetCache(descriptor).Priority;

        /// <summary>Gets type of the command's handler.</summary>
        /// <param name="descriptor">Command descriptor.</param>
        /// <returns>Type of command's handler.</returns>
        public static Type GetHandlerType(this ICommandInstanceDescriptor descriptor)
            => descriptor.Method.DeclaringType;

        /// <summary>Checks if the command is overriding default case sensitivity.</summary>
        /// <remarks>See <see cref="CaseSensitivityAttribute"/> for more information about command case senstivity.</remarks>
        /// <returns>True/false if command is overriding case sensitivity - true to be case sensitive, false to be case insensitive; null if not overriding.</returns>
        public static bool? GetCaseSensitivityOverride(this ICommandInstanceDescriptor descriptor)
            => DescriptorAttributeCache.GetCache(descriptor).CaseSensitivityOverride;

        /// <summary>Gets command's prefix override.</summary>
        /// <param name="descriptor">Command descriptor.</param>
        /// <returns>Prefix value.</returns>
        public static string GetPrefixOverride(this ICommandInstanceDescriptor descriptor)
            => DescriptorAttributeCache.GetCache(descriptor).PrefixOverride;

        /// <summary>Gets command's prefix requirement override.</summary>
        /// <param name="descriptor">Command descriptor.</param>
        /// <returns>Prefix requirement value.</returns>
        public static PrefixRequirement? GetPrefixRequirementOverride(this ICommandInstanceDescriptor descriptor)
            => DescriptorAttributeCache.GetCache(descriptor).PrefixRequirementOverride;

        /// <summary>Gets command's pre-execute checks.</summary>
        /// <param name="descriptor">Command descriptor.</param>
        /// <returns>Enumerable on all pre-execute checks on the command's method and handler type.</returns>
        public static IEnumerable<CommandRequirementAttribute> GetRequirements(this ICommandInstanceDescriptor descriptor)
            => DescriptorAttributeCache.GetCache(descriptor).Requirements;

        /// <summary>Gets display name for given command instance descriptor.</summary>
        /// <param name="descriptor">Command instance descriptor to get the value for.</param>
        /// <remarks>Display name is determined in the following order:<br/>
        /// 1. Value of <see cref="DisplayNameAttribute"/> if present;<br/>
        /// 2. Command text if the command is a standard command (<see cref="CommandAttribute"/>);<br/>
        /// 3. Regex pattern if the command is a regex command (<see cref="RegexCommandAttribute"/>);<br/>
        /// If the name couldn't be determined, null will be returned.</remarks>
        /// <returns>String with determined display name.</returns>
        public static string GetDisplayName(this ICommandInstanceDescriptor descriptor)
            => DescriptorAttributeCache.GetCache(descriptor).DisplayName;

        /// <summary>Gets summary text for given command instance descriptor.</summary>
        /// <param name="descriptor">Command instance descriptor to get the value for.</param>
        /// <returns>Summary text if found on the command; otherwise null.</returns>
        public static string GetSummary(this ICommandInstanceDescriptor descriptor)
            => DescriptorAttributeCache.GetCache(descriptor).Summary;

        /// <summary>Checks if the command or its handler are hidden.</summary>
        /// <param name="descriptor">Command instance descriptor to get the value for.</param>
        /// <returns>True if the command or its handler are marked as hidden; otherwise false.</returns>
        public static bool IsHidden(this ICommandInstanceDescriptor descriptor)
            => DescriptorAttributeCache.GetCache(descriptor).IsHidden;

        /// <summary>Gets help category for given command or its handler.</summary>
        /// <param name="descriptor">Command instance descriptor to get the value for.</param>
        /// <returns>Help Category attribute if found on command or its handler; otherwise null.</returns>
        public static HelpCategoryAttribute GetHelpCategory(this ICommandInstanceDescriptor descriptor)
            => DescriptorAttributeCache.GetCache(descriptor).HelpCategory;

        /// <summary>Gets all custom attributes of specified type.</summary>
        /// <typeparam name="T">Type of attributes.</typeparam>
        /// <param name="descriptor">Command instance descriptor to get the value for.</param>
        /// <param name="includeHandlerAttributes">Whether handler attributes should also be checked.</param>
        /// <returns>Enumerable of found attributes.</returns>
        public static IEnumerable<T> GetAllAttributes<T>(this ICommandInstanceDescriptor descriptor, bool includeHandlerAttributes = false) where T : Attribute
            => DescriptorAttributeCache.GetCache(descriptor).GetAllAttributes<T>(includeHandlerAttributes);

        /// <summary>Gets single custom attribute of specified type.</summary>
        /// <typeparam name="T">Type of attribute.</typeparam>
        /// <param name="descriptor">Command instance descriptor to get the value for.</param>
        /// <param name="includeHandlerAttributes">Whether handler attributes should also be checked.</param>
        /// <returns>Found attribute; null if not found.</returns>
        public static T GetAttribute<T>(this ICommandInstanceDescriptor descriptor, bool includeHandlerAttributes = false) where T : Attribute
            => DescriptorAttributeCache.GetCache(descriptor).GetAttribute<T>(includeHandlerAttributes);
    }
}
