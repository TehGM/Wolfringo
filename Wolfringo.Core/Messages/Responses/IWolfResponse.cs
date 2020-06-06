using Newtonsoft.Json;
using System.Net;

namespace TehGM.Wolfringo.Messages.Responses
{
    /// <summary>Represents Wolf server's response.</summary>
    public interface IWolfResponse
    {
        /// <summary>Response status code.</summary>
        [JsonProperty("code")]
        HttpStatusCode StatusCode { get; }
    }
}
