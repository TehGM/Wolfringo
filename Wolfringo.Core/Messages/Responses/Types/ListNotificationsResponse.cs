using Newtonsoft.Json;
using System.Collections.Generic;

namespace TehGM.Wolfringo.Messages.Responses
{
    public class ListNotificationsResponse : WolfResponse, IWolfResponse
    {
        [JsonProperty("body")]
        public IEnumerable<WolfNotification> Notifications { get; private set; }
    }
}
