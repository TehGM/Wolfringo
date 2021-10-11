using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo.Messages.Responses
{
    /// <summary>Wolf server's response for <see cref="UserAchievementListMessage"/> and <see cref="GroupAchievementListMessage"/>.</summary>
    public class EntityAchievementListResponse : WolfResponse, IWolfResponse
    {
        /// <summary>Entity's achievements IDs and awarded timestamp.</summary>
        [JsonProperty("body")]
        [JsonConverter(typeof(ObjectPropertiesDictionaryConverter<uint, DateTime?>), "id", "additionalInfo.awardedAt")]
        public IReadOnlyDictionary<uint, DateTime?> Achievements { get; private set; }

        /// <summary>Creates a response instance.</summary>
        [JsonConstructor]
        protected EntityAchievementListResponse() : base() { }
    }
}
