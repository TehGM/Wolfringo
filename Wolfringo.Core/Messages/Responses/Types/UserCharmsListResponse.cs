using Newtonsoft.Json;
using System.Collections.Generic;

namespace TehGM.Wolfringo.Messages.Responses
{
    public class UserCharmsListResponse : WolfResponse, IWolfResponse
    {
        [JsonProperty("body")]
        public IEnumerable<WolfCharmSubscription> Charms { get; private set; }
    }
}
