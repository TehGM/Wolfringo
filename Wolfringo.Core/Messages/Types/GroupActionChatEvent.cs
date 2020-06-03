using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TehGM.Wolfringo.Messages
{
    public class GroupActionChatEvent : IChatMessage, IWolfMessage
    {
        private uint? _invokerID;
        private GroupActionType? _type;
        private bool NeedsParsing => _type == null;


        public string Command => MessageCommands.MessageSend;

        // json data
        public string FlightID { get; protected set; }
        public Guid? ID { get; protected set; }
        public bool IsGroupMessage { get; protected set; }
        public string MimeType { get; protected set; }
        public DateTime? Timestamp { get; protected set; }
        public uint? SenderID { get; protected set; }
        public uint RecipientID { get; private set; }

        // binary data
        [JsonIgnore]
        public IReadOnlyCollection<byte> RawData { get; protected set; }
        [JsonIgnore]
        public uint? ActionInvokerID
        {
            get
            {
                if (this.NeedsParsing)
                    this.ParseRawData();
                return _invokerID;
            }
        }
        [JsonIgnore]
        public GroupActionType ActionType
        {
            get
            {
                if (this.NeedsParsing)
                    this.ParseRawData();
                return _type.Value;
            }
        }

        // helper props 
        [JsonIgnore]
        public bool IsPrivateMessage => !this.IsGroupMessage;

        [JsonConstructor]
        protected GroupActionChatEvent()
        {
            this.RawData = new List<byte>();
        }

        protected void ParseRawData()
        {
            if (this.RawData?.Any() != true)
                throw new ArgumentNullException(nameof(this.RawData));

            byte[] data = this.RawData.ToArray();
            int startIndex = data[0] == 4 ? 1 : 0;
            int count = data[0] == 4 ? data.Length - 1 : data.Length;
            JObject actionInfo = JObject.Parse(Encoding.UTF8.GetString(data, startIndex, count));

            // populate props
            this._invokerID = actionInfo["instigatorId"]?.ToObject<uint>();
            this._type = ParseActionType(actionInfo["type"].ToObject<string>());
        }

        private GroupActionType ParseActionType(string type)
        {
            switch (type.ToLower())
            {
                case "join":
                    return GroupActionType.UserJoined;
                case "leave" when this.ActionInvokerID == null:
                    return GroupActionType.UserLeft;
                case "ban":
                    return GroupActionType.Ban;
                case "kick" when this.ActionInvokerID != null:
                    return GroupActionType.Kick;
                case "silence":
                    return GroupActionType.Silence;
                case "reset":
                    return GroupActionType.Reset;
                case "mod":
                    return GroupActionType.Mod;
                case "admin":
                    return GroupActionType.Admin;
                default:
                    throw new ArgumentException($"Unknown group action type: {type}", nameof(type));
            }
        }
    }

    public enum GroupActionType
    {
        UserJoined, // join
        UserLeft,   // leave
        Ban,        // ban
        Kick,       // leave
        Silence,    // silence
        Reset,      // reset
        Mod,        // mod
        Admin       // admin
    }
}
