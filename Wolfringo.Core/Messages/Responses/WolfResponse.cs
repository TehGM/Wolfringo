using Newtonsoft.Json;
using System.Net;

namespace TehGM.Wolfringo.Messages.Responses
{
    public class WolfResponse
    {
        [JsonProperty("code")]
        private int _code;

        public HttpStatusCode ResponseCode => (HttpStatusCode)_code;
    }
}
