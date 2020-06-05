using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace TehGM.Wolfringo
{
    public class WolfAchievement : IWolfEntity, IEquatable<WolfAchievement>
    {
        [JsonProperty("children")]
        public IEnumerable<WolfAchievement> ChildAchievements { get; private set; }
        [JsonProperty("parentId")]
        public uint? ParentID { get; private set; }

        [JsonProperty("id")]
        public uint ID { get; private set; }
        [JsonProperty("name")]
        public string Name { get; private set; }
        [JsonProperty("description")]
        public string Description { get; private set; }
        [JsonProperty("imageUrl")]
        public string ImageURL { get; private set; }

        [JsonProperty("isSecret")]
        public bool IsSecret { get; private set; }
        [JsonProperty("typeId")]
        public int? TypeID { get; private set; }
        [JsonProperty("weight")]
        public int? Weight { get; private set; }
        [JsonProperty("client")]
        public int? Client { get; private set; }

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
