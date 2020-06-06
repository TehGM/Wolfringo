using Newtonsoft.Json;
using System;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo.Messages.Responses
{
    /// <summary>Wolf server's response for <see cref="LoginMessage"/>.</summary>
    public class LoginResponse : WolfResponse, IWolfResponse
    {
        [JsonProperty("offlineMessageTimestamp")]
        [JsonConverter(typeof(MillisecondsEpochConverter))]
        public DateTime OfflineMessageTimestamp { get; private set; }

        /// <summary>Logged in user ID.</summary>
        [JsonProperty("id")]
        public uint UserID { get; private set; }
        /// <summary>Logged in user display name.</summary>
        [JsonProperty("nickname")]
        public string Nickname { get; private set; }
        /// <summary>Logged in user status.</summary>
        [JsonProperty("status")]
        public string UserStatus { get; private set; }
        /// <summary>Logged in user reputation level.</summary>
        [JsonProperty("reputation")]
        public double UserReputation { get; private set; }
        /// <summary>Logged in user email address.</summary>
        [JsonProperty("email")]
        public string UserEmail { get; private set; }
        /// <summary>Logged in user online state.</summary>
        [JsonProperty("onlineState")]
        public WolfOnlineState UserOnlineState { get; private set; }
        /// <summary>Logged in user device type.</summary>
        [JsonProperty("deviceType")]
        public WolfDevice DeviceType { get; private set; }
        /// <summary>Entity state hash.</summary>
        [JsonProperty("hash")]
        public string Hash { get; private set; }

        // TODO: determine more details about these
        [JsonProperty("icon")]
        public int Icon { get; private set; }
        [JsonProperty("privileges")]
        public int Privileges { get; private set; }

        [JsonConstructor]
        protected LoginResponse() : base() { }

        // TODO: not implemented yet: contactListBlockedState, groupMemeberCapabilities, contactListAuthState, charms
    }
}
