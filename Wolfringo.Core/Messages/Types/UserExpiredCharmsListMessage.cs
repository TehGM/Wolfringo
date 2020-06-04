using Newtonsoft.Json;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo.Messages
{
    [ResponseType(typeof(UserCharmsListResponse))]
    public class UserExpiredCharmsListMessage : IWolfMessage
    {
        public string Command => MessageCommands.CharmSubscriberExpiredList;

        [JsonProperty("id")]
        public uint CharmID { get; private set; }

        [JsonConstructor]
        private UserExpiredCharmsListMessage() { }

        public UserExpiredCharmsListMessage(uint userID) : this()
        {
            this.CharmID = userID;
        }
    }
}
