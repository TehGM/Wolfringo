using Newtonsoft.Json;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo.Messages
{
    [ResponseType(typeof(UserCharmsListResponse))]
    public class UserActiveCharmsListMessage : IWolfMessage
    {
        public string Command => MessageCommands.CharmSubscriberActiveList;

        [JsonProperty("id")]
        public uint CharmID { get; private set; }

        [JsonConstructor]
        private UserActiveCharmsListMessage() { }

        public UserActiveCharmsListMessage(uint userID) : this()
        {
            this.CharmID = userID;
        }
    }
}
