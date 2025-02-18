using Newtonsoft.Json;
using System;

namespace TehGM.Wolfringo.Messages.Embeds
{
    /// <summary>Represent a chat embed for image link.</summary>
    public class ImagePreviewChatEmbed : IChatEmbed
    {
        /// <inheritdoc/>
        public string EmbedType => "imagePreview";

        /// <summary>ID of the group to embed.</summary>
        [JsonProperty("url")]
        public string URL { get; }

        /// <summary>Creates a new link preview embed with an image.</summary>
        /// <param name="url">Link to preview.</param>
        public ImagePreviewChatEmbed(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("Image URL is required", nameof(url));

            this.URL = url;
        }
    }
}
