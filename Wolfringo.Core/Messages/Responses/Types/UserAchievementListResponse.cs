using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo.Messages.Responses
{
    /// <summary>Wolf server's response for <see cref="UserAchievementListMessage"/>.</summary>
    public class UserAchievementListResponse : WolfResponse, IWolfResponse
    {
        /// <summary>User's achievements IDs and awarded timestamp.</summary>
        [JsonProperty("body")]
        [JsonConverter(typeof(ObjectPropertiesDictionaryConverter<uint, DateTime>), "achievementId", "updateTime")]
        public IReadOnlyDictionary<uint, DateTime> UserAchievements { get; private set; }

        /// <summary>Creates a response instance.</summary>
        [JsonConstructor]
        protected UserAchievementListResponse() : base() { }
    }
}
