using Newtonsoft.Json;
using System.Collections.Generic;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo
{
    public class WolfCharm : IWolfEntity
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
    }
}
