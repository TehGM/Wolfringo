using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo.Messages.Responses
{
    public class UserAchievementListResponse : WolfResponse, IWolfResponse
    {
        [JsonProperty("body")]
        [JsonConverter(typeof(ObjectPropertiesDictionaryConverter<uint, DateTime>), "achievementId", "updateTime")]
        public IReadOnlyDictionary<uint, DateTime> UserAchievements { get; private set; }
    }
}
