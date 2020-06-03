using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo
{
    public class WolfGroup : IWolfEntity, IEquatable<WolfGroup>
    {
        [JsonProperty("id")]
        public uint ID { get; private set; }
        [JsonProperty("name")]
        public string Name { get; private set; }
        [JsonProperty("hash")]
        public string Hash { get; private set; }
        [JsonProperty("description")]
        public string Description { get; private set; }

        [JsonProperty("owner")]
        [JsonConverter(typeof(EntityIdConverter))]
        public uint OwnerID { get; private set; }
        [JsonProperty("members")]
        public uint MembersCount { get; private set; }
        [JsonIgnore]
        public IReadOnlyDictionary<uint, WolfGroupMember> Members { get; private set; } = new Dictionary<uint, WolfGroupMember>();

        [JsonProperty("official")]
        public bool IsOfficial { get; private set; }
        [JsonProperty("peekable")]
        public bool IsPeekable { get; private set; }
        [JsonProperty("premium")]
        public bool IsPremium { get; private set; }
        [JsonProperty("icon")]
        public int Icon { get; private set; }

        [JsonProperty("audioConfig")]
        public WolfGroupAudioConfig AudioConfig { get; private set; }
        [JsonProperty("audioCounts")]
        public WolfGroupAudioCounts AudioCounts { get; private set; }

        // data from "extended" prop
        [JsonProperty("advancedAdmin", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsExtendedAdminEnabled { get; private set; }
        [JsonProperty("discoverable", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsDiscoverable { get; private set; }
        [JsonProperty("entryLevel", NullValueHandling = NullValueHandling.Ignore)]
        public int? EntryReputationLevel { get; private set; }
        [JsonProperty("language", NullValueHandling = NullValueHandling.Ignore)]
        public WolfLanguage? Language { get; private set; }
        [JsonProperty("locked", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsLocked { get; private set; }
        [JsonProperty("passworded", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsPassworded { get; private set; }
        [JsonProperty("questionable", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsQuestionable { get; private set; }
        [JsonProperty("longDescription", NullValueHandling = NullValueHandling.Ignore)]
        public string LongDescription { get; private set; }

        public override bool Equals(object obj)
            => Equals(obj as WolfGroup);

        public bool Equals(WolfGroup other)
            => other != null && ID == other.ID && Hash == other.Hash;

        public override int GetHashCode()
            => 1213502048 + ID.GetHashCode();

        public static bool operator ==(WolfGroup left, WolfGroup right)
            => EqualityComparer<WolfGroup>.Default.Equals(left, right);

        public static bool operator !=(WolfGroup left, WolfGroup right)
            => !(left == right);

        public class WolfGroupAudioConfig
        {
            [JsonProperty("enabled")]
            public bool IsEnabled { get; private set; }
            [JsonProperty("minRepLevel")]
            public double MinimumReputationLevel { get; private set; }
            [JsonProperty("id")]
            public uint GroupID { get; private set; }
            [JsonProperty("stageId")]
            public WolfStageType? StageType { get; private set; }
        }

        public class WolfGroupAudioCounts
        {
            [JsonProperty("id")]
            public uint GroupID { get; private set; }
            [JsonProperty("broadcasterCount")]
            public int BroadcastersCount { get; private set; }
            [JsonProperty("consumerCount")]
            public int ListenersCount { get; private set; }
        }
    }
}
