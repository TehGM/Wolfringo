using Newtonsoft.Json;
using System.Net;

namespace TehGM.Wolfringo.Messages.Responses
{
    public interface IWolfResponse
    {
        [JsonProperty("code")]
        HttpStatusCode StatusCode { get; }
    }
}
