using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using TehGM.Wolfringo.Messages;
using TehGM.Wolfringo.Messages.Embeds;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo.Utilities.Internal
{
    /// <summary>Internal utility class for generating <see cref="IChatEmbed"/> from extracted URL metadata.</summary>
    /// <remarks>Class created to avoid duplication between Commands system extensions and Sender utility extensions.</remarks>
    internal static class ChatEmbedBuilder
    {
        public static async Task<IEnumerable<IChatEmbed>> BuildEmbedsAsync(IWolfClient client, IEnumerable<ChatMessageFormatting.GroupLinkData> groupLinks, IEnumerable<ChatMessageFormatting.LinkData> urlLinks, ChatMessageSendingOptions options, CancellationToken cancellationToken)
        {
            if (options.EnableGroupLinkPreview && groupLinks.Any())
                return new IChatEmbed[] { new GroupPreviewChatEmbed(groupLinks.First().GroupID) };

            if ((options.EnableWebsiteLinkPreview || options.EnableImageLinkPreview) && urlLinks.Any())
            {
                try
                {
                    string url = urlLinks.First().URL;
                    UrlMetadataResponse metadataResponse = await client.SendAsync<UrlMetadataResponse>(new UrlMetadataMessage(url), cancellationToken).ConfigureAwait(false);
                    if (metadataResponse.IsSuccess() && !metadataResponse.IsBlacklisted)
                    {
                        if (metadataResponse.ImageSize != null && options.EnableImageLinkPreview)
                            return new IChatEmbed[] { new ImagePreviewChatEmbed(metadataResponse.ImageURL) };
                        if (options.EnableWebsiteLinkPreview)
                        {
                            string linkTitle = string.IsNullOrWhiteSpace(metadataResponse.Title) ? "-" : metadataResponse.Title;
                            return new IChatEmbed[] { new LinkPreviewChatEmbed(linkTitle, url) };
                        }
                    }
                }
                catch (MessageSendingException) { }
            }

            return Enumerable.Empty<IChatEmbed>();
        }
    }
}
