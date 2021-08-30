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
        /// 1. Value of <see cref="DisplayNameAttribute"/> if present;
        /// 2. Command text if the command is a standard command (<see cref="CommandAttribute"/>);
        /// 3. Regex pattern if the command is a regex command (<see cref="RegexCommandAttribute"/>);
        /// 4. Fallback to Method name for custom command types.</remarks>
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
