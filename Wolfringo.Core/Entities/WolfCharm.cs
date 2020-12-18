using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo
{
    /// <summary>Wolf Charm.</summary>
    public class WolfCharm : IWolfEntity, IEquatable<WolfCharm>
    {
        /// <summary>Charm ID.</summary>
        [JsonProperty("id")]
        public uint ID { get; private set; }
        /// <summary>URL of charm image.</summary>
        [JsonProperty("imageUrl")]
        public string ImageURL { get; private set; }
        /// <summary>Code name of the charm.</summary>
        [JsonProperty("name")]
        public string Name { get; private set; }

        /// <summary>Charm weight. (?)</summary>
        [JsonProperty("weight")]
        public int Weight { get; private set; }
        /// <summary>Product ID of this charm.</summary>
        [JsonProperty("productId")]
        public uint? ProductID { get; private set; }

        /// <summary>Charm's translated names.</summary>
        [JsonProperty("nameTranslationList")]
        [JsonConverter(typeof(ObjectPropertiesDictionaryConverter<WolfLanguage, string>), "languageId", "text")]
        public IReadOnlyDictionary<WolfLanguage, string> TranslatedNames { get; private set; }
        /// <summary>Charm's translated descriptions.</summary>
        [JsonProperty("descriptionList")]
        [JsonConverter(typeof(ObjectPropertiesDictionaryConverter<WolfLanguage, string>), "languageId", "text")]
        public IReadOnlyDictionary<WolfLanguage, string> TranslatedDescriptions { get; private set; }
        /// <summary>?</summary>
        [JsonProperty("descriptionPhraseId")]
        public uint? DescriptionPhraseID { get; private set; }

        /// <summary>Creates a new instance of this WOLF entity.</summary>
        [JsonConstructor]
        protected WolfCharm() { }

        /// <inheritdoc/>
        public override bool Equals(object obj)
            => Equals(obj as WolfCharm);

        /// <inheritdoc/>
        public bool Equals(WolfCharm other)
            => other != null && ID == other.ID && string.Equals(this.Name, other.Name, StringComparison.OrdinalIgnoreCase);

        /// <inheritdoc/>
        public override int GetHashCode()
            => 1213502048 + ID.GetHashCode();

        /// <inheritdoc/>
        public static bool operator ==(WolfCharm left, WolfCharm right)
            => EqualityComparer<WolfCharm>.Default.Equals(left, right);

        /// <inheritdoc/>
        public static bool operator !=(WolfCharm left, WolfCharm right)
            => !(left == right);
    }
}
