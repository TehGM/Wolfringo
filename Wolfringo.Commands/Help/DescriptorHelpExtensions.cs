using System;
using System.Collections.Generic;
using TehGM.Wolfringo.Commands.Initialization;

namespace TehGM.Wolfringo.Commands.Help
{
    /// <summary>Set of extension methods for <see cref="ICommandInstanceDescriptor"/> designed to help with creating help list.</summary>
    /// <remarks>Extensions in this class will automatically cache results to improve performance.</remarks>
    public static class DescriptorHelpExtensions
    {
        private static readonly IDictionary<ICommandInstanceDescriptor, DescriptorHelpCache> _cache = new Dictionary<ICommandInstanceDescriptor, DescriptorHelpCache>();

        /// <summary>Gets display name for given command instance descriptor.</summary>
        /// <param name="descriptor">Command instance descriptor to get the value for.</param>
        /// <remarks>Display name is determined in the following order:<br/>
        /// 1. Value of <see cref="DisplayNameAttribute"/> if present;<br/>
        /// 2. Command text if the command is a standard command (<see cref="CommandAttribute"/>);<br/>
        /// 3. Regex pattern if the command is a regex command (<see cref="RegexCommandAttribute"/>);<br/>
        /// If the name couldn't be determined, null will be returned.</remarks>
        /// <returns>String with determined display name.</returns>
        public static string GetDisplayName(this ICommandInstanceDescriptor descriptor)
            => GetCache(descriptor).DisplayName;

        /// <summary>Gets summary text for given command instance descriptor.</summary>
        /// <param name="descriptor">Command instance descriptor to get the value for.</param>
        /// <returns>Summary text if found on the command; otherwise null.</returns>
        public static string GetSummary(this ICommandInstanceDescriptor descriptor)
            => GetCache(descriptor).Summary;

        /// <summary>Checks if the command or its handler are hidden.</summary>
        /// <param name="descriptor">Command instance descriptor to get the value for.</param>
        /// <returns>True if the command or its handler are marked as hidden; otherwise false.</returns>
        public static bool IsHidden(this ICommandInstanceDescriptor descriptor)
            => GetCache(descriptor).IsHidden;

        /// <summary>Gets help category for given command or its handler.</summary>
        /// <param name="descriptor">Command instance descriptor to get the value for.</param>
        /// <returns>Help Category attribute if found on command or its handler; otherwise null.</returns>
        public static HelpCategoryAttribute GetHelpCategory(this ICommandInstanceDescriptor descriptor)
            => GetCache(descriptor).HelpCategory;

        /// <summary>Gets all custom attributes of specified type.</summary>
        /// <typeparam name="T">Type of attributes.</typeparam>
        /// <param name="descriptor">Command instance descriptor to get the value for.</param>
        /// <param name="includeHandlerAttributes">Whether handler attributes should also be checked.</param>
        /// <returns>Enumerable of found attributes.</returns>
        public static IEnumerable<T> GetAllAttributes<T>(this ICommandInstanceDescriptor descriptor, bool includeHandlerAttributes = false) where T : Attribute
            => GetCache(descriptor).GetAllAttributes<T>(includeHandlerAttributes);

        /// <summary>Gets single custom attribute of specified type.</summary>
        /// <typeparam name="T">Type of attribute.</typeparam>
        /// <param name="descriptor">Command instance descriptor to get the value for.</param>
        /// <param name="includeHandlerAttributes">Whether handler attributes should also be checked.</param>
        /// <returns>Found attribute; null if not found.</returns>
        public static T GetAttribute<T>(this ICommandInstanceDescriptor descriptor, bool includeHandlerAttributes = false) where T : Attribute
            => GetCache(descriptor).GetAttribute<T>(includeHandlerAttributes);

        private static DescriptorHelpCache GetCache(ICommandInstanceDescriptor descriptor)
        {
            if (_cache.TryGetValue(descriptor, out DescriptorHelpCache result))
                return result;
            result = new DescriptorHelpCache(descriptor);
            _cache.Add(descriptor, result);
            return result;
        }
    }
}
