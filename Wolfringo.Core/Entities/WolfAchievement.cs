using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace TehGM.Wolfringo
{
    /// <summary>A Wolf achievement.</summary>
    public class WolfAchievement : IWolfEntity, IEquatable<WolfAchievement>
    {
        /// <summary>Child achievements, if any.</summary>
        [JsonProperty("children")]
        public IEnumerable<WolfAchievement> ChildAchievements { get; private set; }
        /// <summary>ID of parent achievement, if any.</summary>
        [JsonProperty("parentId")]
        public uint? ParentID { get; private set; }

        /// <summary>ID of the achievement.</summary>
        [JsonProperty("id")]
        public uint ID { get; private set; }
        /// <summary>Localized name of the achievement.</summary>
        [JsonProperty("name")]
        public string Name { get; private set; }
        /// <summary>Localized description of the achievement.</summary>
        [JsonProperty("description")]
        public string Description { get; private set; }
        /// <summary>URL of achievement's image.</summary>
        [JsonProperty("imageUrl")]
        public string ImageURL { get; private set; }

        /// <summary>Is this a secret achievement?</summary>
        [JsonProperty("isSecret")]
        public bool IsSecret { get; private set; }
        [JsonProperty("typeId")]
        public int? TypeID { get; private set; }
        [JsonProperty("weight")]
        public int? Weight { get; private set; }
        [JsonProperty("client")]
        public int? Client { get; private set; }

        [JsonConstructor]
        protected WolfAchievement() { }

        public override bool Equals(object obj)
            => Equals(obj as WolfAchievement);

        public bool Equals(WolfAchievement other)
            => other != null && ID == other.ID;

        public override int GetHashCode()
            => 1213502048 + ID.GetHashCode();

        public static bool operator ==(WolfAchievement left, WolfAchievement right)
            => EqualityComparer<WolfAchievement>.Default.Equals(left, right);

        public static bool operator !=(WolfAchievement left, WolfAchievement right)
            => !(left == right);
    }
}
