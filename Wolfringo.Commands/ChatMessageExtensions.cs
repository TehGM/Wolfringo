using System;
using TehGM.Wolfringo.Messages;

namespace TehGM.Wolfringo.Commands
{
    public static class ChatMessageExtensions
    {
        public static bool MatchesPrefixRequirement(this ChatMessage message, string prefix, PrefixRequirement requirement, bool caseInsensitive, out int startIndex)
        {
            StringComparison comparison = caseInsensitive ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            bool startsWithPrefix = message.Text.StartsWith(prefix, comparison);
            startIndex = startsWithPrefix ? prefix.Length : 0;
            // if it does start with prefix, we know for sure it matches requirement
            if (startsWithPrefix)
                return true;
            // now we know it does NOT start with prefix
            // so let's check if prefix is required in group messages if the message is in group...
            if (message.IsGroupMessage && (requirement & PrefixRequirement.Group) == PrefixRequirement.Group)
                return false;
            // ... or in private messages if the message is in group
            if (message.IsPrivateMessage && (requirement & PrefixRequirement.Private) == PrefixRequirement.Private)
                return false;
            // if we got here, we know that the prefix is not required
            return true;
        }

        public static bool MatchesPrefixRequirement(this ChatMessage message, ICommandsOptions options, out int startIndex)
            => MatchesPrefixRequirement(message, options.Prefix, options.RequirePrefix, options.CaseInsensitive, out startIndex);
    }
}
