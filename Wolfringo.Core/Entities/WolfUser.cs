using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace TehGM.Wolfringo
{
    public class WolfUser : IWolfEntity, IEquatable<WolfUser>
    {
        [JsonProperty("id")]
        public uint ID { get; private set; }
        [JsonProperty("hash", NullValueHandling = NullValueHandling.Ignore)]
        public string Hash { get; private set; }
        [JsonProperty("nickname")]
        public string Username { get; private set; }
        [JsonProperty("status")]
        public string Status { get; private set; }
        [JsonProperty("email", NullValueHandling = NullValueHandling.Ignore)]
        public string Email { get; private set; }

        [JsonProperty("reputation")]
        public double Reputation { get; private set; }
        [JsonProperty("deviceType")]
        public WolfDevice Device { get; private set; }
        [JsonProperty("icon")]
        public int Icon { get; private set; }
        [JsonProperty("onlineState")]
        public WolfOnlineState OnlineState { get; private set; }
        [JsonProperty("privileges")]
        public int Privileges { get; private set; }

        public override bool Equals(object obj)
            => Equals(obj as WolfUser);

        public bool Equals(WolfUser other)
            => other != null && ID == other.ID && Hash == other.Hash;

        public override int GetHashCode()
            => 1213502048 + ID.GetHashCode();

        public static bool operator ==(WolfUser left, WolfUser right)
            => EqualityComparer<WolfUser>.Default.Equals(left, right);

        public static bool operator !=(WolfUser left, WolfUser right)
            => !(left == right);
    }
}
