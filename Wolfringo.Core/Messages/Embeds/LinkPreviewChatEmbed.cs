using Newtonsoft.Json;
using System;

namespace TehGM.Wolfringo.Messages.Embeds
{
    /// <summary>Represent a chat embed for website link.</summary>
    public class LinkPreviewChatEmbed : IChatEmbed
    {
        /// <inheritdoc/>
        public string EmbedType => "linkPreview";

        /// <summary>Title of the webpage.</summary>
        [JsonProperty("title")]
        public string Title { get; set; }
        /// <summary>URL of the webpage.</summary>
        [JsonProperty("url")]
        public string URL { get; }

        /// <summary>Creates a new link preview embed.</summary>
        /// <param name="title">Title of the webpage.</param>
        /// <param name="url">Link to preview.</param>
        public LinkPreviewChatEmbed(string title, string url)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Link title is required", nameof(title));
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("Link URL is required", nameof(url));

            this.Title = title;
            this.URL = url;
        }
    }
}
