using Newtonsoft.Json;
using System;

namespace TehGM.Wolfringo
{
    /// <summary>Wolf notification.</summary>
    public class WolfNotification : IWolfEntity
    {
        /// <summary>ID of the notification.</summary>
        [JsonProperty("id")]
        public uint ID { get; private set; }
        /// <summary>Notification's start time.</summary>
        [JsonProperty("startAt")]
        public DateTime StartTime { get; private set; }
        /// <summary>Notification's end time.</summary>
        [JsonProperty("endAt")]
        public DateTime EndTime { get; private set; }

        /// <summary>Notification's title.</summary>
        [JsonProperty("title")]
        public string Title { get; private set; }
        /// <summary>Notification's message text.</summary>
        [JsonProperty("message")]
        public string Message { get; private set; }
        /// <summary>Notification's link.</summary>
        [JsonProperty("link")]
        public string Link { get; private set; }
        /// <summary>URL to notification's image.</summary>
        [JsonProperty("imageUrl")]
        public string ImageURL { get; private set; }

        /// <summary>Is this notification favourited?</summary>
        [JsonProperty("favourite")]
        public bool IsFavourite { get; private set; }
        /// <summary>Is this notification global?</summary>
        [JsonProperty("global")]
        public bool IsGlobal { get; private set; }
        /// <summary>Is this notification persistent?</summary>
        [JsonProperty("persistent")]
        public bool IsPersistent { get; private set; }

        /// <summary>Layout type of the notification.</summary>
        [JsonProperty("layoutType")]
        public int LayoutType { get; private set; }
        /// <summary>Type of news stream.</summary>
        [JsonProperty("newsStreamType")]
        public int NewsStreamType { get; private set; }
        /// <summary>Notification type.</summary>
        [JsonProperty("type")]
        public int Type { get; private set; }

        /// <summary>Creates a new instance.</summary>
        [JsonConstructor]
        protected WolfNotification() { }
    }
}
