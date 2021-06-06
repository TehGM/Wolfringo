using Newtonsoft.Json;

namespace TehGM.Wolfringo.Messages.Responses
{
    /// <summary>Response for <see cref="UrlMetadataMessage"/> containing URL metadata.</summary>
    public class UrlMetadataResponse : WolfResponse, IWolfResponse
    {
        /// <summary>Title of the URL website. Null for image links.</summary>
        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; private set; }
        /// <summary>Description of the URL website. Can be null in various situations.</summary>
        /// <remarks>This value might be null if the URL is an image link, but also when website offers no description metadata etc.</remarks>
        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; private set; }
        /// <summary>Is official link according to WOLF?</summary>
        [JsonProperty("isOfficial")]
        public bool IsOfficial { get; private set; }
        /// <summary>URL domain.</summary>
        [JsonProperty("domain")]
        public string Domain { get; private set; }
        /// <summary>URL of embed image. Null if website contains no image.</summary>
        [JsonProperty("imageUrl", NullValueHandling = NullValueHandling.Ignore)]
        public string ImageURL { get; private set; }
        /// <summary>Size of embed image in bytes. Null if website contains no image.</summary>
        [JsonProperty("imageSize", NullValueHandling = NullValueHandling.Ignore)]
        public uint? ImageSize { get; private set; }
        /// <summary>Whether the URL is blacklisted by WOLF servers.</summary>
        [JsonProperty("isBlacklisted")]
        public bool IsBlacklisted { get; private set; }

        /// <summary>Creates a response instance.</summary>
        [JsonConstructor]
        protected UrlMetadataResponse() : base() { }
    }
}
