using Newtonsoft.Json;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo.Messages.Types
{
    [ResponseType(typeof(ListNotificationsResponse))]
    public class ListNotificationsMessage : IWolfMessage
    {
        public string Command => MessageCommands.NotificationList;

        private const WolfLanguage _defaultLanguage = WolfLanguage.English;

        [JsonProperty("language")]
        public WolfLanguage Language { get; private set; }
        [JsonProperty("deviceType")]
        public WolfDevice Device { get; private set; }

        public ListNotificationsMessage(WolfLanguage language = _defaultLanguage, WolfDevice device = WolfDevice.Bot)
        {
            this.Language = language;
            this.Device = device;
        }

        public ListNotificationsMessage(WolfDevice device)
            : this(_defaultLanguage, device: device) { }
    }
}
