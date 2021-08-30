using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TehGM.Wolfringo.Commands.Initialization;

namespace TehGM.Wolfringo.Commands.Help
{
    /// <summary>Set of extension methods for <see cref="ICommandInstanceDescriptor"/> designed to help with creating help list.</summary>
    /// <remarks>Extensions in this class will check if the descriptor is <see cref="CommandInstanceDescriptor"/> - if so, they'll take advantage of its cached data for performance.</remarks>
    public static class DescriptorHelpExtensions
    {
        /// <summary>Gets display name for given command instance descriptor.</summary>
        /// <param name="descriptor">Command instance descriptor to get the value for.</param>
        /// <remarks>Display name is determined in the following order:<br/>
        /// 1. Value of <see cref="DisplayNameAttribute"/> if present;
        /// 2. Command text if the command is a standard command (<see cref="CommandAttribute"/>);
        /// 3. Regex pattern if the command is a regex command (<see cref="RegexCommandAttribute"/>);
        /// 4. Fallback to Method name for custom command types.</remarks>
        /// <returns>String with determined display name.</returns>
        public static string GetDisplayName(this ICommandInstanceDescriptor descriptor)
        {
            DisplayNameAttribute displayNameAttribute = GetAttribute<DisplayNameAttribute>(descriptor, false);
            if (displayNameAttribute != null)
                return displayNameAttribute.Text;

            if (descriptor.Attribute is CommandAttribute command)
                return command.Text;
            if (descriptor.Attribute is RegexCommandAttribute regex)
                return regex.Pattern;
            return descriptor.Method.Name;
        }

        /// <summary>Gets summary text for given command instance descriptor.</summary>
        /// <param name="descriptor">Command instance descriptor to get the value for.</param>
        /// <returns>Summary text if found on the command; otherwise null.</returns>
        public static string GetSummary(this ICommandInstanceDescriptor descriptor)
            => GetAttribute<SummaryAttribute>(descriptor, false)?.Text;

        /// <summary>Checks if the command or its handler are hidden.</summary>
        /// <param name="descriptor">Command instance descriptor to get the value for.</param>
        /// <returns>True if the command or its handler are marked as hidden; otherwise false.</returns>
        public static bool IsHidden(this ICommandInstanceDescriptor descriptor)
        {
            HiddenAttribute hiddenAttribute = GetAttribute<HiddenAttribute>(descriptor, true);
            return hiddenAttribute != null;
        }

        /// <summary>Gets help category for given command or its handler.</summary>
        /// <param name="descriptor">Command instance descriptor to get the value for.</param>
        /// <returns>Help Category attribute if found on command or its handler; otherwise null.</returns>
        public static HelpCategoryAttribute GetHelpCategory(this ICommandInstanceDescriptor descriptor)
        {
            HelpCategoryAttribute helpCategoryAttribute = GetAttribute<HelpCategoryAttribute>(descriptor, true);
            return helpCategoryAttribute;
        }

        /// <summary>Gets all custom attributes of specified type on given command instance descriptor.</summary>
        /// <typeparam name="T">Type of attributes.</typeparam>
        /// <param name="descriptor">Command instance descriptor to get the attributes for.</param>
        /// <param name="includeHandlerAttributes">Whether handler attributes should also be checked.</param>
        /// <returns>Enumerable of found attributes.</returns>
        public static IEnumerable<T> GetAllAttributes<T>(this ICommandInstanceDescriptor descriptor, bool includeHandlerAttributes = false) where T : Attribute
        {
            IEnumerable<T> attributes = null;
            if (descriptor is CommandInstanceDescriptor defaultDescriptor)
                attributes = defaultDescriptor.AllAttributes?.Where(attr => attr is T).Cast<T>() ?? Enumerable.Empty<T>();
            else
                attributes = descriptor.Method.GetCustomAttributes<T>(true);
            if (!includeHandlerAttributes)
                return attributes;

            // union handler attributes BEFORE the method attributes
            // this will ensure that GetAttribute will prioritize method attributes (LastOrDefault())
            IEnumerable<T> handlerAttributes = descriptor.Method.DeclaringType.GetCustomAttributes<T>(true);
            return handlerAttributes.Union(attributes);
        }

        /// <summary>Gets single custom attribute of specified type on given command instance descriptor.</summary>
        /// <typeparam name="T">Type of attribute.</typeparam>
        /// <param name="descriptor">Command instance descriptor to get the attribute for.</param>
        /// <param name="includeHandlerAttributes">Whether handler attributes should also be checked.</param>
        /// <returns>Found attribute; null if not found.</returns>
        public static T GetAttribute<T>(this ICommandInstanceDescriptor descriptor, bool includeHandlerAttributes = false) where T : Attribute
            => GetAllAttributes<T>(descriptor, includeHandlerAttributes).LastOrDefault();
    }
}
