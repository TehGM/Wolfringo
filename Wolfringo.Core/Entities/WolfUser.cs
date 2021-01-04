using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo
{
    /// <summary>Wolf user.</summary>
    public class WolfUser : IWolfEntity, IEquatable<WolfUser>
    {
        /// <summary>ID of the user.</summary>
        [JsonProperty("id")]
        public uint ID { get; private set; }
        /// <summary>Entity state hash.</summary>
        [JsonProperty("hash", NullValueHandling = NullValueHandling.Ignore)]
        public string Hash { get; private set; }
        /// <summary>User's display name.</summary>
        [JsonProperty("nickname")]
        public string Nickname { get; private set; }
        /// <summary>User's status.</summary>
        [JsonProperty("status")]
        public string Status { get; private set; }


        /// <summary>User's reputation level.</summary>
        [JsonProperty("reputation")]
        public double Reputation { get; private set; }
        /// <summary>User's current device.</summary>
        [JsonProperty("deviceType")]
        public WolfDevice Device { get; private set; }
        /// <summary>User icon ID.</summary>
        [JsonProperty("icon")]
        public int Icon { get; private set; }
        /// <summary>User's online state.</summary>
        [JsonProperty("onlineState")]
        public WolfOnlineState OnlineState { get; private set; }
        /// <summary>User's privileges.</summary>
        [JsonProperty("privileges")]
        public WolfPrivilege Privileges { get; private set; }


        /// <summary>User's selected active charm.</summary>
        [JsonProperty("charms")]
        [JsonConverter(typeof(ValueOrPropertyConverter), "selectedList[0].charmId")]
        public uint? ActiveCharmID { get; private set; }

        // data from "extended" prop - should be populated by converter
        /// <summary>User's name, as specified in the profile.</summary>
        /// <remarks>If this value is null, user request with extended data might be required.</remarks>
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string ProfileName { get; private set; }
        /// <summary>User's "About Me" description.</summary>
        /// <remarks>If this value is null, user request with extended data might be required.</remarks>
        [JsonProperty("about", NullValueHandling = NullValueHandling.Ignore)]
        public string About { get; private set; }
        /// <summary>User's language.</summary>
        /// <remarks>If this value is null, user request with extended data is required.</remarks>
        [JsonProperty("language", NullValueHandling = NullValueHandling.Ignore)]
        public WolfLanguage? Language { get; private set; }
        /// <summary>User's gender.</summary>
        /// <remarks>If this value is null, user request with extended data is required.</remarks>
        [JsonProperty("gender", NullValueHandling = NullValueHandling.Ignore)]
        public WolfGender? Gender { get; private set; }
        /// <summary>User's looking for.</summary>
        /// <remarks>If this value is null, user request with extended data is required.</remarks>
        [JsonProperty("lookingFor", NullValueHandling = NullValueHandling.Ignore)]
        public WolfLookingFor? LookingFor { get; private set; }
        /// <summary>User's relationship status.</summary>
        /// <remarks>If this value is null, user request with extended data is required.</remarks>
        [JsonProperty("relationship", NullValueHandling = NullValueHandling.Ignore)]
        public WolfRelationship? Relationship { get; private set; }
        /// <summary>User's links.</summary>
        /// <remarks>If this value is null, user request with extended data might be required.</remarks>
        [JsonProperty("urls", NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<string> Links { get; private set; }
        /// <summary>User's timezone offset.</summary>
        /// <remarks>If this value is null, user request with extended data is required.</remarks>
        [JsonProperty("utcOffset", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(MinutesTimespanConverter))]
        public TimeSpan? UtcOffset { get; private set; }
        /// <summary>User's date of birth.</summary>
        /// <remarks>If this value is null, user request with extended data is required.</remarks>
        [JsonProperty("dateOfBirth", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? DateOfBirth { get; private set; }

        /// <summary>Creates a new instance of WOLF user.</summary>
        [JsonConstructor]
        protected WolfUser() { }

        /// <inheritdoc/>
        public override bool Equals(object obj)
            => Equals(obj as WolfUser);

        /// <inheritdoc/>
        public bool Equals(WolfUser other)
            => other != null && ID == other.ID && Hash == other.Hash;

        /// <inheritdoc/>
        public override int GetHashCode()
            => 1213502048 + ID.GetHashCode();

        /// <inheritdoc/>
        public static bool operator ==(WolfUser left, WolfUser right)
            => EqualityComparer<WolfUser>.Default.Equals(left, right);

        /// <inheritdoc/>
        public static bool operator !=(WolfUser left, WolfUser right)
            => !(left == right);
    }
}
