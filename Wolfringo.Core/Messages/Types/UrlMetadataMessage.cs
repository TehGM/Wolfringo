using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>A message for requesting metadata about a link as seen by WOLF servers.</summary>
    /// <remarks>Uses <see cref="TipDetailsResponse"/> as response type.</remarks>
    [ResponseType(typeof(UrlMetadataResponse))]
    public class UrlMetadataMessage : IWolfMessage, IHeadersWolfMessage
    {
        /// <inheritdoc/>
        [JsonIgnore]
        public IDictionary<string, object> Headers { get; } = new Dictionary<string, object>()
        {
            { "version", 2 }
        };

        /// <inheritdoc/>
        /// <remarks>Equals to <see cref="MessageEventNames.MetadataUrl"/>.</remarks>
        [JsonIgnore]
        public string EventName => MessageEventNames.MetadataUrl;

        /// <summary>URL to request the metadata of.</summary>
        [JsonProperty("url")]
        public string URL { get; private set; }

        /// <summary>Creates a message instance.</summary>
        [JsonConstructor]
        protected UrlMetadataMessage() { }

        /// <summary>Creates a message instance.</summary>
        /// <param name="url">URL to request the metadata of.</param>
        /// <exception cref="ArgumentNullException">URL is null.</exception>
        /// <exception cref="ArgumentException">URL is empty, whitespace, or otherwise invalid.</exception>
        public UrlMetadataMessage(string url)
        {
            if (url == null)
                throw new ArgumentNullException(nameof(url));
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("Link URL cannot be empty or whitespace", nameof(url));
            this.URL = url;
        }
    }
}
