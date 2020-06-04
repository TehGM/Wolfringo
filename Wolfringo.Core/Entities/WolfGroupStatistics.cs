using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo
{
    public class WolfGroupStatistics
    {
        // group info
        [JsonProperty("id")]
        public uint GroupID { get; private set; }
        [JsonProperty("name")]
        public string GroupName { get; private set; }
        [JsonProperty("owner")]
        [JsonConverter(typeof(ValueOrPropertyConverter), "subId")]
        public uint OwnerID { get; private set; }

        // group stats
        [JsonProperty("wordCount")]
        public int WordsCount { get; private set; }
        [JsonProperty("textCount")]
        public int TextMessagesCount { get; private set; }
        [JsonProperty("lineCount")]
        public int TextLinesCount { get; private set; }
        [JsonProperty("questionCount")]
        public int QuestionsCount { get; private set; }
        [JsonProperty("swearCount")]
        public int SwearsCount { get; private set; }
        [JsonProperty("imageCount")]
        public int ImagesCount { get; private set; }
        [JsonProperty("actionCount")]
        public int ActionsCount { get; private set; }
        [JsonProperty("voiceCount")]
        public int VoiceMessagesCount { get; private set; }
        [JsonProperty("emoticonCount")]
        public int EmoticonsCount { get; private set; }
        [JsonProperty("happyCount")]
        public int HappyEmoticonsCount { get; private set; }
        [JsonProperty("packCount")]
        public int PacksCount { get; private set; }
        [JsonProperty("spokenCount")]
        public int SpokenCount { get; private set; }
        [JsonProperty("memberCount")]
        public int MembersCount { get; private set; }
        [JsonProperty("timestamp")]
        [JsonConverter(typeof(EpochConverter))]
        public DateTime Timestamp { get; private set; }

        // trends
        [JsonProperty("trendsHour")]
        public IEnumerable<HourlyTrend> HourOfDayTrends { get; private set; }
        [JsonProperty("trendsDay")]
        public IEnumerable<DailyTrend> DayOfWeekTrends { get; private set; }
        [JsonProperty("trends")]
        public IEnumerable<DailyTrend> RecentDaysTrends { get; private set; }

        // top members general
        [JsonProperty("top25")]
        public IEnumerable<MemberStats> TopMembers { get; private set; }
        [JsonProperty("next30")]
        public IEnumerable<MemberStats> TopRunnerUps { get; private set; }

        // breakdown stats
        [JsonProperty("topWord")]
        public IEnumerable<WordPerLineStat> TopWordSenders { get; private set; }
        [JsonProperty("topText")]
        public IEnumerable<TextMessageStat> TopTextSenders { get; private set; }
        [JsonProperty("topQuestion")]
        public IEnumerable<TextMessageStat> TopQuestionSenders { get; private set; }
        [JsonProperty("topEmoticon")]
        public IEnumerable<TextMessageStat> TopEmoticonSenders { get; private set; }
        [JsonProperty("topHappy")]
        public IEnumerable<TextMessageStat> TopHappyEmoticonSenders { get; private set; }
        [JsonProperty("topSad")]
        public IEnumerable<TextMessageStat> TopSadEmoticonSenders { get; private set; }
        [JsonProperty("topSwear")]
        public IEnumerable<TextMessageStat> TopSwearSenders { get; private set; }
        [JsonProperty("topImage")]
        public IEnumerable<MessageStat> TopImageSenders { get; private set; }
        [JsonProperty("topAction")]
        public IEnumerable<MessageStat> TopActionSenders { get; private set; }

        public interface ITrend
        {
            [JsonProperty("lineCount")]
            int LinesCount { get; }
        }

        public class HourlyTrend : ITrend
        {
            [JsonProperty("hour")]
            public int Hour { get; private set; }
            public int LinesCount { get; private set; }
        }

        public class DailyTrend : ITrend
        {
            [JsonProperty("day")]
            public int Day { get; private set; }
            public int LinesCount { get; private set; }
        }

        public interface IMemberStat
        {
            [JsonProperty("subId")]
            uint UserID { get; }
            [JsonProperty("nickname")]
            string UserNickname { get; }
        }

        public class MemberStats : IMemberStat
        {
            public uint UserID { get; private set; }
            public string UserNickname { get; private set; }
            [JsonProperty("message")]
            public string UserStatus { get; private set; }

            [JsonProperty("wordCount")]
            public int WordsCount { get; private set; }
            [JsonProperty("textCount")]
            public int TextMessagesCount { get; private set; }
            [JsonProperty("lineCount")]
            public int TextLinesCount { get; private set; }
            [JsonProperty("questionCount")]
            public int QuestionsCount { get; private set; }
            [JsonProperty("swearCount")]
            public int SwearsCount { get; private set; }
            [JsonProperty("imageCount")]
            public int ImagesCount { get; private set; }
            [JsonProperty("actionCount")]
            public int ActionsCount { get; private set; }
            [JsonProperty("voiceCount")]
            public int VoiceMessagesCount { get; private set; }
            [JsonProperty("emoticonCount")]
            public int EmoticonsCount { get; private set; }
            [JsonProperty("happyCount")]
            public int HappyEmoticonsCount { get; private set; }
            [JsonProperty("packCount")]
            public int PacksCount { get; private set; }
        }

        public class WordPerLineStat : IMemberStat
        {
            public uint UserID { get; private set; }
            public string UserNickname { get; private set; }
            [JsonProperty("wpl")]
            // this is botched, and returns the same as text message percentage
            public double WordsPerLine { get; private set; }
        }

        public class MessageStat : IMemberStat
        {
            public uint UserID { get; private set; }
            public string UserNickname { get; private set; }

            [JsonProperty("percentage")]
            public double Percentage { get; private set; }
        }

        public class TextMessageStat : MessageStat, IMemberStat
        {
            [JsonProperty("randomQuote")]
            public string RandomQuote { get; private set; }
        }
    }
}
