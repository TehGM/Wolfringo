using Newtonsoft.Json;
using System.Collections.Generic;

namespace TehGM.Wolfringo.Messages.Responses
{
    public class ListCharmsResponse : WolfResponse, IWolfResponse
    {
        [JsonProperty("body")]
        public IEnumerable<WolfCharm> Charms { get; private set; }
    }
}
