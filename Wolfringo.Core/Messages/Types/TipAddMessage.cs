using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>Message and event for adding a tip, and receiving notifications of messages being tipped.</summary>
    public class TipAddMessage : IWolfMessage
    {
        [JsonIgnore]
        public string Command => MessageCommands.TipAdd;

        /// <summary>Context type of the tip.</summary>
        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter), true)]
        public WolfTip.ContextType ContextType { get; private set; }
        /// <summary>ID of message to tip.</summary>
        [JsonProperty("id")]
        public long MessageID { get; private set; }
        /// <summary>List of tips.</summary>
        [JsonProperty("charmList")]
        public IEnumerable<WolfTip> Tips { get; private set; }
        /// <summary>ID of the group where the message is in.</summary>
        [JsonProperty("groupId")]
        public uint GroupID { get; private set; }
        /// <summary>Author of the message.</summary>
        [JsonProperty("subscriberId", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public uint MessageAuthorID { get; private set; }


        /// <summary>The person who tipped.</summary>
        [JsonProperty("sourceSubscriberId", NullValueHandling = NullValueHandling.Ignore)]
        public uint? TipperID { get; private set; }
        [JsonProperty("version", NullValueHandling = NullValueHandling.Ignore)]
        public int? Version { get; private set; }

        [JsonConstructor]
        protected TipAddMessage() { }

        /// <summary>Creates a message instance.</summary>
        /// <param name="messageID">ID (timestamp) of the message to tip.</param>
        /// <param name="groupID">ID of the group where the message is in.</param>
        /// <param name="authorID">ID of the message author.</param>
        /// <param name="contextType">Context type of the tip.</param>
        /// <param name="tips">Tips to send.</param>
        public TipAddMessage(long messageID, uint groupID, uint authorID, WolfTip.ContextType contextType, IEnumerable<WolfTip> tips)
        {
            if (tips?.Any() != true)
                throw new ArgumentException("Must request at least one tip to add", nameof(tips));
            this.MessageID = messageID;
            this.MessageAuthorID = authorID;
            this.GroupID = groupID;
            this.ContextType = contextType;
            this.Tips = new ReadOnlyCollection<WolfTip>((tips as IList<WolfTip>) ?? tips.ToArray());
        }
    }
}
