using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using TehGM.Wolfringo.Messages.Serialization.Internal;

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

        // data from "extended" prop - should be populated by converter
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; private set; }
        [JsonProperty("about", NullValueHandling = NullValueHandling.Ignore)]
        public string About { get; private set; }
        [JsonProperty("language", NullValueHandling = NullValueHandling.Ignore)]
        public WolfLanguage? Language { get; private set; }
        [JsonProperty("gender", NullValueHandling = NullValueHandling.Ignore)]
        public WolfGender? Gender { get; private set; }
        [JsonProperty("lookingFor", NullValueHandling = NullValueHandling.Ignore)]
        public WolfLookingFor? LookingFor { get; private set; }
        [JsonProperty("urls", NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<string> Links { get; private set; }
        [JsonProperty("utcOffset", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(MinutesTimespanConverter))]
        public TimeSpan? UtcOffset { get; private set; }

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
