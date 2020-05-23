using Newtonsoft.Json;
using System;

namespace TehGM.Wolfringo.Messages.Responses
{
    public class LoginResponse : WolfResponse
    {
        [JsonProperty("offlineMessageTimestamp")]
        public DateTime OfflineMessageTimestamp { get; private set; }

        [JsonProperty("id")]
        public uint UserID { get; private set; }
        [JsonProperty("nickname")]
        public string Username { get; private set; }
        [JsonProperty("status")]
        public string UserStatus { get; private set; }
        [JsonProperty("reputation")]
        public double UserReputation { get; private set; }
        [JsonProperty("email")]
        public string UserEmail { get; private set; }

        // TODO: determine more details about these
        [JsonProperty("onlineState")]
        public int UserOnlineState { get; private set; }
        [JsonProperty("icon")]
        public int Icon { get; private set; }
        [JsonProperty("deviceType")]
        public int DeviceType { get; private set; }
        [JsonProperty("hash")]
        public string Hash { get; private set; }
        [JsonProperty("privileges")]
        public int Privileges { get; private set; }

        // TODO: not implemented yet: contactListBlockedState, groupMemeberCapabilities, contactListAuthState, charms
    }
}
