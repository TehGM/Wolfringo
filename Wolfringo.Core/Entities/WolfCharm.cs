using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo
{
    public class WolfCharm : IWolfEntity, IEquatable<WolfCharm>
    {
        [JsonProperty("id")]
        public uint ID { get; private set; }
        [JsonProperty("imageUrl")]
        public string ImageURL { get; private set; }
        [JsonProperty("name")]
        public string Name { get; private set; }

        [JsonProperty("weight")]
        public int Weight { get; private set; }
        [JsonProperty("productId")]
        public uint? ProductID { get; private set; }

        [JsonProperty("nameTranslationList")]
        [JsonConverter(typeof(ObjectPropertiesDictionaryConverter<WolfLanguage, string>), "languageId", "text")]
        public IReadOnlyDictionary<WolfLanguage, string> TranslatedNames { get; private set; }
        [JsonProperty("descriptionList")]
        [JsonConverter(typeof(ObjectPropertiesDictionaryConverter<WolfLanguage, string>), "languageId", "text")]
        public IReadOnlyDictionary<WolfLanguage, string> TranslatedDescriptions { get; private set; }
        [JsonProperty("descriptionPhraseId")]
        public uint? DescriptionPhraseID { get; private set; }

        public override bool Equals(object obj)
            => Equals(obj as WolfCharm);

        public bool Equals(WolfCharm other)
            => other != null && ID == other.ID && string.Equals(this.Name, other.Name, StringComparison.OrdinalIgnoreCase);

        public override int GetHashCode()
            => 1213502048 + ID.GetHashCode();

        public static bool operator ==(WolfCharm left, WolfCharm right)
            => EqualityComparer<WolfCharm>.Default.Equals(left, right);

        public static bool operator !=(WolfCharm left, WolfCharm right)
            => !(left == right);
    }
}
