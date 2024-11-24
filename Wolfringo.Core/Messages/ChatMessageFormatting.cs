using Newtonsoft.Json;
using System.Collections.Generic;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>Formatting metadata for outgoing chat messages.</summary>
    public class ChatMessageFormatting
    {
        /// <summary>Group links present in the text.</summary>
        [JsonProperty("groupLinks", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<GroupLinkData> GroupLinks { get; }
        /// <summary>Web links present in the text.</summary>
        [JsonProperty("links", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<LinkData> Links { get; }

        /// <summary>Creates a new instance of the formatting metadata.</summary>
        /// <param name="groupLinks">Group links present in the text.</param>
        /// <param name="links">Web links present in the text.</param>
        public ChatMessageFormatting(IEnumerable<GroupLinkData> groupLinks, IEnumerable<LinkData> links)
        {
            this.GroupLinks = groupLinks;
            this.Links = links;
        }

        /// <summary>Represents position in text and related group ID for a group link.</summary>
        public class GroupLinkData
        {
            /// <summary>Where the link begins in the text.</summary>
            [JsonProperty("start")]
            public uint Start { get; }
            /// <summary>Where the link ends in the text.</summary>
            [JsonProperty("end")]
            public uint End { get; }
            /// <summary>ID of the linked group.</summary>
            [JsonProperty("groupId")]
            public uint GroupID { get; }

            /// <summary>Creates a new instance of group link metadata.</summary>
            /// <param name="start">Where the link begins in the text.</param>
            /// <param name="end">Where the link ends in the text.</param>
            /// <param name="groupID">ID of the linked group.</param>
            public GroupLinkData(uint start, uint end, uint groupID)
            {
                this.Start = start;
                this.End = end;
                this.GroupID = groupID;
            }
        }

        /// <summary>Represents position in text and URL for a link.</summary>
        public class LinkData
        {
            /// <summary>Where the link begins in the text.</summary>
            [JsonProperty("start")]
            public uint Start { get; }
            /// <summary>Where the link ends in the text.</summary>
            [JsonProperty("end")]
            public uint End { get; }
            /// <summary>The URL value.</summary>
            [JsonProperty("url")]
            public string URL { get; }

            /// <summary>Creates a new instance of group link metadata.</summary>
            /// <param name="start">Where the link begins in the text.</param>
            /// <param name="end">Where the link ends in the text.</param>
            /// <param name="url">The URL value.</param>
            public LinkData(uint start, uint end, string url)
            {
                this.Start = start;
                this.End = end;
                this.URL = url;
            }
        }
    }
}
