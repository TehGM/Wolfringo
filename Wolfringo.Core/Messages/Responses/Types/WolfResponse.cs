using Newtonsoft.Json;
using System.Net;

namespace TehGM.Wolfringo.Messages.Responses
{
    /// <inheritdoc/>
    public class WolfResponse : IWolfResponse
    {
        /// <inheritdoc/>
        [JsonProperty("code")]
        public HttpStatusCode StatusCode { get; private set; }
        /// <summary>Nested error code.</summary>
        [JsonProperty("subCode", NullValueHandling = NullValueHandling.Ignore)]
        public WolfErrorCode? ErrorCode { get; private set; }
        /// <summary>Nested error message.</summary>
        [JsonProperty("subMessage", NullValueHandling = NullValueHandling.Ignore)]
        public string ErrorMessage { get; private set; }
    }
}
