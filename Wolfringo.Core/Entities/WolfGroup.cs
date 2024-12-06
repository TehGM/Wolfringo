using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo
{
    /// <summary>Wolf group.</summary>
    public class WolfGroup : IWolfEntity, IEquatable<WolfGroup>
    {
        /// <summary>ID of the group.</summary>
        [JsonProperty("id")]
        public uint ID { get; private set; }
        /// <summary>Group public name.</summary>
        [JsonProperty("name")]
        public string Name { get; private set; }
        /// <summary>Entity state hash.</summary>
        [JsonProperty("hash")]
        public string Hash { get; private set; }
        /// <summary>Group's short description.</summary>
        [JsonProperty("description")]
        public string Description { get; private set; }

        /// <summary>ID of group owner.</summary>
        [JsonProperty("owner")]
        [JsonConverter(typeof(EntityIdConverter))]
        public uint OwnerID { get; private set; }
        /// <summary>Group members.</summary>
        [JsonIgnore]
        public IReadOnlyDictionary<uint, WolfGroupMember> Members { get; private set; } = new Dictionary<uint, WolfGroupMember>();

        /// <summary>Group's reputation level.</summary>
        [JsonProperty("reputation")]
        public double Reputation { get; private set; }
        /// <summary>Is this group official?</summary>
        [JsonProperty("official")]
        public bool IsOfficial { get; private set; }
        /// <summary>Is message history visible even if user is not in the group?</summary>
        [JsonProperty("peekable")]
        public bool IsPeekable { get; private set; }
        /// <summary>Is group premium?</summary>
        [JsonProperty("premium")]
        public bool IsPremium { get; private set; }
        /// <summary>Group icon ID.</summary>
        [JsonProperty("icon")]
        public int? Icon { get; private set; }

        /// <summary>Group's audio configuration.</summary>
        [JsonProperty("audioConfig")]
        public WolfGroupAudioConfig AudioConfig { get; private set; }
        /// <summary>Group's audio channel active members count.</summary>
        [JsonProperty("audioCounts")]
        public WolfGroupAudioCounts AudioCounts { get; private set; }

        // data from "extended" prop
        /// <summary>Is extended admin privilege enabled in this group?</summary>
        /// <remarks>If this value is null, group request with extended data is required.</remarks>
        [JsonProperty("advancedAdmin", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsExtendedAdminEnabled { get; private set; }
        /// <summary>Is group publicly discoverable?</summary>
        /// <remarks>If this value is null, group request with extended data is required.</remarks>
        [JsonProperty("discoverable", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsDiscoverable { get; private set; }
        /// <summary>Group's entry reputation level.</summary>
        /// <remarks>If this value is null, group request with extended data is required.</remarks>
        [JsonProperty("entryLevel", NullValueHandling = NullValueHandling.Ignore)]
        public int? EntryReputationLevel { get; private set; }
        /// <summary>Language of the group.</summary>
        /// <remarks>If this value is null, group request with extended data is required.</remarks>
        [JsonProperty("language", NullValueHandling = NullValueHandling.Ignore)]
        public WolfLanguage? Language { get; private set; }
        /// <summary>Is group locked?</summary>
        /// <remarks>If this value is null, group request with extended data is required.</remarks>
        [JsonProperty("locked", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsLocked { get; private set; }
        /// <summary>Is group password protected?</summary>
        /// <remarks>If this value is null, group request with extended data is required.</remarks>
        [JsonProperty("passworded", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsPassworded { get; private set; }
        /// <summary>Is group marked as questionable?</summary>
        /// <remarks>If this value is null, group request with extended data is required.</remarks>
        [JsonProperty("questionable", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsQuestionable { get; private set; }
        /// <summary>Long description of the group.</summary>
        /// <remarks>If this value is null, group request with extended data might be required.</remarks>
        [JsonProperty("longDescription", NullValueHandling = NullValueHandling.Ignore)]
        public string LongDescription { get; private set; }

        /// <summary>Creates a new instance of WOLF group object.</summary>
        [JsonConstructor]
        protected WolfGroup() { }

        /// <inheritdoc/>
        public override bool Equals(object obj)
            => Equals(obj as WolfGroup);

        /// <inheritdoc/>
        public bool Equals(WolfGroup other)
            => other != null && ID == other.ID && Hash == other.Hash;

        /// <inheritdoc/>
        public override int GetHashCode()
            => 1213502048 + ID.GetHashCode();

        /// <inheritdoc/>
        public static bool operator ==(WolfGroup left, WolfGroup right)
            => EqualityComparer<WolfGroup>.Default.Equals(left, right);

        /// <inheritdoc/>
        public static bool operator !=(WolfGroup left, WolfGroup right)
            => !(left == right);

        /// <summary>Group audio configuration.</summary>
        public class WolfGroupAudioConfig
        {
            /// <summary>Is audio stage enabled?</summary>
            [JsonProperty("enabled")]
            public bool IsEnabled { get; private set; }
            /// <summary>Minimum reputation level to enter audio stage.</summary>
            [JsonProperty("minRepLevel")]
            public int? MinimumReputationLevel { get; private set; }
            /// <summary>ID of the group.</summary>
            [JsonProperty("id")]
            public uint GroupID { get; private set; }
            /// <summary>Type of the stage.</summary>
            [JsonProperty("stageId")]
            public WolfStageType? StageType { get; private set; }

            /// <summary>Creates a new instance of WOLF group audio config object.</summary>
            [JsonConstructor]
            protected WolfGroupAudioConfig() { }
        }

        /// <summary>Group audio channel active members count.</summary>
        public class WolfGroupAudioCounts
        {
            /// <summary>ID of the group.</summary>
            [JsonProperty("id")]
            public uint GroupID { get; private set; }
            /// <summary>Count of users currently broadcasting.</summary>
            [JsonProperty("broadcasterCount")]
            public int BroadcastersCount { get; private set; }
            /// <summary>Count of users currently listening.</summary>
            [JsonProperty("consumerCount")]
            public int ListenersCount { get; private set; }

            /// <summary>Creates a new instance of WOLF group audio counts object.</summary>
            [JsonConstructor]
            protected WolfGroupAudioCounts() { }
        }
    }
}
