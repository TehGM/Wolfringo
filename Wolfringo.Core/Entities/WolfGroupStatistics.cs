using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo
{
    /// <summary>Group stats.</summary>
    public class WolfGroupStatistics
    {
        // group info
        /// <summary>ID of the group.</summary>
        [JsonProperty("id")]
        public uint GroupID { get; private set; }
        /// <summary>Name of the group.</summary>
        [JsonProperty("name")]
        public string GroupName { get; private set; }
        /// <summary>ID of the group owner.</summary>
        [JsonProperty("owner")]
        [JsonConverter(typeof(ValueOrPropertyConverter), "subId")]
        public uint OwnerID { get; private set; }

        // group stats
        /// <summary>Words per line.</summary>
        /// <remarks>This statistic appears to be currently broken, and have the same values as <see cref="TextMessagesCount"/>.</remarks>
        [JsonProperty("wordCount")]
        public int WordsCount { get; private set; }
        /// <summary>Count of text messages.</summary>
        [JsonProperty("textCount")]
        public int TextMessagesCount { get; private set; }
        /// <summary>Count of text lines.</summary>
        [JsonProperty("lineCount")]
        public int TextLinesCount { get; private set; }
        /// <summary>Count of messages with questions.</summary>
        [JsonProperty("questionCount")]
        public int QuestionsCount { get; private set; }
        /// <summary>Count of messages with swears.</summary>
        [JsonProperty("swearCount")]
        public int SwearsCount { get; private set; }
        /// <summary>Count of images posted.</summary>
        [JsonProperty("imageCount")]
        public int ImagesCount { get; private set; }
        /// <summary>Count of group actions.</summary>
        [JsonProperty("actionCount")]
        public int ActionsCount { get; private set; }
        /// <summary>Count of voice messages.</summary>
        [JsonProperty("voiceCount")]
        public int VoiceMessagesCount { get; private set; }
        /// <summary>Count of messages with emotes.</summary>
        [JsonProperty("emoticonCount")]
        public int EmoticonsCount { get; private set; }
        /// <summary>Count of messages with happy emotes.</summary>
        [JsonProperty("happyCount")]
        public int HappyEmoticonsCount { get; private set; }
        /// <summary>Count of messages with sad emotes.</summary>
        [JsonProperty("sadCount")]
        public int SadEmoticonsCount { get; private set; }
        [JsonProperty("packCount")]
        public int PacksCount { get; private set; }
        [JsonProperty("spokenCount")]
        public int SpokenCount { get; private set; }
        /// <summary>Group member count.</summary>
        [JsonProperty("memberCount")]
        public int MembersCount { get; private set; }
        /// <summary>Time at which these stats were generated.</summary>
        [JsonProperty("timestamp")]
        [JsonConverter(typeof(EpochConverter))]
        public DateTime Timestamp { get; private set; }

        // trends
        /// <summary>Messages posted per hour of day (0-23).</summary>
        [JsonProperty("trendsHour")]
        public IEnumerable<HourlyTrend> HourOfDayTrends { get; private set; }
        /// <summary>Messages posted per day of week (0-6).</summary>
        [JsonProperty("trendsDay")]
        public IEnumerable<DailyTrend> DayOfWeekTrends { get; private set; }
        /// <summary>Messages posted in recent days.</summary>
        [JsonProperty("trends")]
        public IEnumerable<DailyTrend> RecentDaysTrends { get; private set; }

        // top members general
        /// <summary>Top 25 active members.</summary>
        [JsonProperty("top25")]
        public IEnumerable<MemberStats> TopMembers { get; private set; }
        /// <summary>Next top 30 active members.</summary>
        [JsonProperty("next30")]
        public IEnumerable<MemberStats> TopRunnerUps { get; private set; }

        // breakdown stats
        /// <summary>Members sending most words.</summary>
        /// <remarks>This statistic appears to be currently broken, and have the same values as <see cref="TopTextSenders"/>.</remarks>
        [JsonProperty("topWord")]
        public IEnumerable<WordPerLineStat> TopWordSenders { get; private set; }
        /// <summary>Members sending text messages.</summary>
        [JsonProperty("topText")]
        public IEnumerable<TextMessageStat> TopTextSenders { get; private set; }
        /// <summary>Members sending most messages with questions.</summary>
        [JsonProperty("topQuestion")]
        public IEnumerable<TextMessageStat> TopQuestionSenders { get; private set; }
        /// <summary>Members sending most messages with emoticons.</summary>
        [JsonProperty("topEmoticon")]
        public IEnumerable<TextMessageStat> TopEmoticonSenders { get; private set; }
        /// <summary>Members sending most messages with happy emoticons.</summary>
        [JsonProperty("topHappy")]
        public IEnumerable<TextMessageStat> TopHappyEmoticonSenders { get; private set; }
        /// <summary>Members sending most messages with sad emoticons.</summary>
        [JsonProperty("topSad")]
        public IEnumerable<TextMessageStat> TopSadEmoticonSenders { get; private set; }
        /// <summary>Members sending most messages with swaers.</summary>
        [JsonProperty("topSwear")]
        public IEnumerable<TextMessageStat> TopSwearSenders { get; private set; }
        /// <summary>Members sending most images.</summary>
        [JsonProperty("topImage")]
        public IEnumerable<MessageStat> TopImageSenders { get; private set; }
        /// <summary>Members performing most admin actions..</summary>
        [JsonProperty("topAction")]
        public IEnumerable<MessageStat> TopActionSenders { get; private set; }

        /// <summary>Group statistics trend.</summary>
        public interface ITrend
        {
            /// <summary>Count of lines in given timespan.</summary>
            [JsonProperty("lineCount")]
            int LinesCount { get; }
        }

        /// <summary>Messages trend per hour.</summary>
        public class HourlyTrend : ITrend
        {
            /// <summary>Hour timespan.</summary>
            [JsonProperty("hour")]
            public int Hour { get; private set; }
            /// <inheritdoc/>
            public int LinesCount { get; private set; }
        }

        /// <summary>Messages trend per day.</summary>
        public class DailyTrend : ITrend
        {
            /// <summary>Day timespan.</summary>
            [JsonProperty("day")]
            public int Day { get; private set; }
            /// <inheritdoc/>
            public int LinesCount { get; private set; }
        }

        /// <summary>Group member statistic.</summary>
        public interface IMemberStat
        {
            /// <summary>Member's user ID.</summary>
            [JsonProperty("subId")]
            uint UserID { get; }
            /// <summary>Member's name.</summary>
            [JsonProperty("nickname")]
            string UserNickname { get; }
        }

        /// <summary>Group member's statistics.</summary>
        public class MemberStats : IMemberStat
        {
            /// <inheritdoc/>
            public uint UserID { get; private set; }
            /// <inheritdoc/>
            public string UserNickname { get; private set; }
            /// <summary>Member's status.</summary>
            [JsonProperty("message")]
            public string UserStatus { get; private set; }

            /// <summary>Words per line.</summary>
            /// <remarks>This statistic appears to be currently broken, and have the same values as <see cref="TextMessagesCount"/>.</remarks>
            [JsonProperty("wordCount")]
            public int WordsCount { get; private set; }
            /// <summary>Count of text messages.</summary>
            [JsonProperty("textCount")]
            public int TextMessagesCount { get; private set; }
            /// <summary>Count of text lines.</summary>
            [JsonProperty("lineCount")]
            public int TextLinesCount { get; private set; }
            /// <summary>Count of messages with questions.</summary>
            [JsonProperty("questionCount")]
            public int QuestionsCount { get; private set; }
            /// <summary>Count of messages with swears.</summary>
            [JsonProperty("swearCount")]
            public int SwearsCount { get; private set; }
            /// <summary>Count of images posted.</summary>
            [JsonProperty("imageCount")]
            public int ImagesCount { get; private set; }
            /// <summary>Count of group actions.</summary>
            [JsonProperty("actionCount")]
            public int ActionsCount { get; private set; }
            /// <summary>Count of voice messages.</summary>
            [JsonProperty("voiceCount")]
            public int VoiceMessagesCount { get; private set; }
            /// <summary>Count of messages with emotes.</summary>
            [JsonProperty("emoticonCount")]
            public int EmoticonsCount { get; private set; }
            /// <summary>Count of messages with happy emotes.</summary>
            [JsonProperty("happyCount")]
            public int HappyEmoticonsCount { get; private set; }
            /// <summary>Count of messages with sad emotes.</summary>
            [JsonProperty("sadCount")]
            public int SadEmoticonsCount { get; private set; }
            [JsonProperty("packCount")]
            public int PacksCount { get; private set; }
        }

        /// <summary>Word-per-line quantified statistic.</summary>
        public class WordPerLineStat : IMemberStat
        {
            /// <inheritdoc/>
            public uint UserID { get; private set; }
            /// <inheritdoc/>
            public string UserNickname { get; private set; }
            /// <summary>Words per line.</summary>
            /// <remarks>This statistic appears to be currently broken, and have the same values as <see cref="TextMessagesCount"/>.</remarks>
            [JsonProperty("wpl")]
            // this is botched, and returns the same as text message percentage
            public double WordsPerLine { get; private set; }
        }

        /// <summary>Message quantified statistic.</summary>
        public class MessageStat : IMemberStat
        {
            /// <inheritdoc/>
            public uint UserID { get; private set; }
            /// <inheritdoc/>
            public string UserNickname { get; private set; }
            /// <summary>Percentage of all messages.</summary>
            [JsonProperty("percentage")]
            public double Percentage { get; private set; }
        }

        /// <summary>Text message quantified statistic.</summary>
        public class TextMessageStat : MessageStat, IMemberStat
        {
            /// <summary>Random related message.</summary>
            [JsonProperty("randomQuote")]
            public string RandomQuote { get; private set; }
        }
    }
}
