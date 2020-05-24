using Newtonsoft.Json;
using System;

namespace TehGM.Wolfringo
{
    public class WolfNotification : IWolfEntity
    {
        [JsonProperty("id")]
        public uint ID { get; private set; }
        [JsonProperty("startAt")]
        public DateTime StartTime { get; private set; }
        [JsonProperty("endAt")]
        public DateTime EndTime { get; private set; }

        [JsonProperty("title")]
        public string Title { get; private set; }
        [JsonProperty("message")]
        public string Message { get; private set; }
        [JsonProperty("link")]
        public string Link { get; private set; }
        [JsonProperty("imageUrl")]
        public string ImageURL { get; private set; }

        [JsonProperty("favourite")]
        public bool IsFavourite { get; private set; }
        [JsonProperty("global")]
        public bool IsGlobal { get; private set; }
        [JsonProperty("persistent")]
        public bool IsPersistent { get; private set; }

        [JsonProperty("layoutType")]
        public int LayoutType { get; private set; }
        [JsonProperty("newsStreamType")]
        public int NewsStreamType { get; private set; }
        [JsonProperty("type")]
        public int Type { get; private set; }
    }
}
