using System;
using TehGM.Wolfringo.Messages;

namespace TehGM.Wolfringo.Commands
{
    /// <summary>Extensions for ChatMessage to be used by Commands System.</summary>
    public static class ChatMessageExtensions
    {
        /// <summary>Checks if the message matches prefix requirements for command processing.</summary>
        /// <param name="message">Message to check.</param>
        /// <param name="prefix">Prefix.</param>
        /// <param name="requirement">Prefix requirement to check.</param>
        /// <param name="caseSensitive">Whether the check should be performed case-sensitively.</param>
        /// <param name="startIndex">Index of start of actual command message, without prefix.</param>
        /// <returns>True if the message matches prefix requirement; otherwise false.</returns>
        public static bool MatchesPrefixRequirement(this ChatMessage message, string prefix, PrefixRequirement requirement, bool caseSensitive, out int startIndex)
        {
            startIndex = GetPrefixStartIndex(message, prefix, caseSensitive);
            // if it does start with prefix, we know for sure it matches requirement
            if (startIndex != -1)
                return true;
            // now we know it does NOT start with prefix
            // so let's check if prefix is required in group messages if the message is in group...
            if (message.IsGroupMessage && (requirement & PrefixRequirement.Group) == PrefixRequirement.Group)
                return false;
            // ... or in private messages if the message is in group
            if (message.IsPrivateMessage && (requirement & PrefixRequirement.Private) == PrefixRequirement.Private)
                return false;
            // if we got here, we know that the prefix is not required
            startIndex = 0;
            return true;
        }

        /// <summary>Checks if the message matches prefix requirements for command processing.</summary>
        /// <param name="message">Message to check.</param>
        /// <param name="options">Commands options to perform the check with.</param>
        /// <param name="startIndex">Index of start of actual command message, without prefix.</param>
        /// <returns>True if the message matches prefix requirement; otherwise false.</returns>
        public static bool MatchesPrefixRequirement(this ChatMessage message, ICommandOptions options, out int startIndex)
            => MatchesPrefixRequirement(message, options.Prefix, options.RequirePrefix, options.CaseSensitivity, out startIndex);

        /// <summary>Gets message text without prefix.</summary>
        /// <param name="message">The received message.</param>
        /// <param name="prefix">Prefix.</param>
        /// <param name="caseSensitive">Whether the check should be performed case-sensitively.</param>
        /// <returns>The message with prefix stripped off.</returns>
        public static string GetMessageWithoutPrefix(this ChatMessage message, string prefix, bool caseSensitive)
        {
            int startIndex = GetPrefixStartIndex(message, prefix, caseSensitive);
            if (startIndex == -1 || startIndex == 0)
                return message.Text;
            else
                return message.Text.Substring(startIndex, message.Text.Length - startIndex);
        }

        /// <summary>Gets message text without prefix.</summary>
        /// <param name="message">The received message.</param>
        /// <param name="options">Commands options.</param>
        /// <returns>The message with prefix stripped off.</returns>
        public static string GetMessageWithoutPrefix(this ChatMessage message, ICommandOptions options)
            => GetMessageWithoutPrefix(message, options.Prefix, options.CaseSensitivity);

        private static int GetPrefixStartIndex(ChatMessage message, string prefix, bool caseSensitive)
        {
            if (string.IsNullOrEmpty(prefix))
                return 0;

            StringComparison comparison = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
            bool startsWithPrefix = message.Text.StartsWith(prefix, comparison);
            return startsWithPrefix ? prefix.Length : -1;
        }
    }
}
