using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using TehGM.Wolfringo.Messages;
using TehGM.Wolfringo.Messages.Responses;
using TehGM.Wolfringo.Caching;

namespace TehGM.Wolfringo.Utilities.Internal
{
    /// <summary>Internal utility helper for detecting and building group links in outgoing messages.</summary>
    public static class GroupLinkDetectionHelper
    {
        /// <summary>Finds group links in the text, and builds metadata for each found link.</summary>
        /// <param name="client">Client to use when retrieving profiles of unknown groups.</param>
        /// <param name="text">Text to find group links in.</param>
        /// <param name="cancellationToken">Token to cancel operation.</param>
        /// <returns>Built metadata with information about group links to be used when sending a message.</returns>
        public static async Task<IEnumerable<ChatMessageFormatting.GroupLinkData>> FindGroupLinksAsync(IWolfClient client, string text, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(text))
                return Array.Empty<ChatMessageFormatting.GroupLinkData>();

            List<ChatMessageFormatting.GroupLinkData> foundGroupLinks = null;

            foreach (GroupLinkPosition position in FindGroupLinksInText(text).Distinct())
            {
                try
                {
                    WolfGroup group = await GetGroupFromCacheOrServerAsync(client, position.Text, cancellationToken).ConfigureAwait(false);
                    if (group != null)
                    {
                        if (foundGroupLinks == null)
                            foundGroupLinks = new List<ChatMessageFormatting.GroupLinkData>();

                        foundGroupLinks.Add(new ChatMessageFormatting.GroupLinkData((uint)position.Start, (uint)position.End, group.ID));
                    }
                }
                catch (MessageSendingException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }

                cancellationToken.ThrowIfCancellationRequested();
            }

            return foundGroupLinks.AsEnumerable() ?? Array.Empty<ChatMessageFormatting.GroupLinkData>();
        }

        private static async Task<WolfGroup> GetGroupFromCacheOrServerAsync(IWolfClient client, string groupName, CancellationToken cancellationToken)
        {
            if (client is IWolfClientCacheAccessor cacheAccessor)
            {
                WolfGroup result = cacheAccessor.GetCachedGroup(groupName);
                if (result != null)
                    return result;
            }

            GroupProfileResponse response = await client.SendAsync<GroupProfileResponse>(
                        new GroupProfileMessage(groupName, GroupProfileMessage.DefaultRequestEntities, true),
                        cancellationToken).ConfigureAwait(false);
            return response?.GroupProfiles?.FirstOrDefault() ?? null;
        }

        /// <summary>Enumerates all group links found in the text.</summary>
        /// <param name="text">Text to find group links in.</param>
        /// <returns>Names and positions of groups found in the text.</returns>
        public static IEnumerable<GroupLinkPosition> FindGroupLinksInText(string text)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                int openIndex = text.IndexOf('[');
                while (openIndex > -1)
                {
                    int closeIndex = text.IndexOf(']', openIndex);
                    if (closeIndex < 0)
                        break;

                    int length = closeIndex - openIndex - 1;
                    string groupName = text.Substring(openIndex + 1, length);
                    if (!string.IsNullOrWhiteSpace(groupName))
                    {
                        yield return new GroupLinkPosition(openIndex, closeIndex + 1, groupName);
                    }

                    openIndex = text.IndexOf('[', closeIndex);
                }
            }
        }
    }
}
