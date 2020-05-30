using Newtonsoft.Json;
using System.Net;

namespace TehGM.Wolfringo.Messages.Responses
{
    public class WolfResponse : IWolfResponse
    {
        [JsonProperty("code")]
        public HttpStatusCode StatusCode { get; private set; }
        [JsonProperty("subCode", NullValueHandling = NullValueHandling.Ignore)]
        public WolfErrorCode? ErrorCode { get; private set; }
        [JsonProperty("subMessage", NullValueHandling = NullValueHandling.Ignore)]
        public string ErrorMessage { get; private set; }
    }
}
