using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>A message for requesting data of specific achievements.</summary>
    /// <remarks>Uses <see cref="AchievementResponse"/> as response type.</remarks>
    [ResponseType(typeof(AchievementResponse))]
    public class AchievementMessage : IWolfMessage, IHeadersWolfMessage
    {
        /// <inheritdoc/>
        /// <remarks>Equals to <see cref="MessageEventNames.Achievement"/>.</remarks>
        [JsonIgnore]
        public string EventName => MessageEventNames.Achievement;

        /// <summary>Language of achievement translations.</summary>
        [JsonProperty("languageId")]
        public WolfLanguage Language { get; private set; }
        /// <summary>IDs of achievements to request.</summary>
        [JsonProperty("idList")]
        public IEnumerable<uint> RequestAchievementIDs { get; private set; }

        /// <inheritdoc/>
        [JsonIgnore]
        public IDictionary<string, object> Headers { get; } = new Dictionary<string, object>()
        {
            { "version", 2 }
        };

        /// <summary>Creates a message instance.</summary>
        [JsonConstructor]
        protected AchievementMessage() { }

        /// <summary>Creates a message instance.</summary>
        /// <param name="language">Language of achievements.</param>
        /// <param name="achievementIDs">IDs of achievements to request.</param>
        public AchievementMessage(WolfLanguage language, IEnumerable<uint> achievementIDs)
        {
            this.Language = language;

            if (achievementIDs?.Any() != true)
                throw new ArgumentException("Must request at least one achievement ID.", nameof(achievementIDs));
            this.RequestAchievementIDs = new ReadOnlyCollection<uint>((achievementIDs as IList<uint>) ?? achievementIDs.ToArray());
        }
    }
}
