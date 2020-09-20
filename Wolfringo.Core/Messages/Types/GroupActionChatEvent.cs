using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>A group action embedded in chat.</summary>
    /// <seealso cref="ChatMessage"/>
    /// <seealso cref="GroupAdminMessage"/>
    public class GroupActionChatEvent : IChatMessage, IWolfMessage
    {
        private uint? _invokerID;
        private GroupActionType? _type;
        private bool NeedsParsing => _type == null;


        /// <inheritdoc/>
        [JsonIgnore]
        public string Command => MessageCommands.MessageSend;

        // json data
        /// <inheritdoc/>
        public string FlightID { get; private set; }
        /// <inheritdoc/>
        public Guid? ID { get; private set; }
        /// <inheritdoc/>
        public bool IsGroupMessage { get; private set; }
        /// <inheritdoc/>
        public string MimeType { get; private set; }
        /// <inheritdoc/>
        public WolfTimestamp? Timestamp { get; private set; }
        /// <inheritdoc/>
        public uint? SenderID { get; private set; }
        /// <inheritdoc/>
        public uint RecipientID { get; private set; }

        // binary data
        /// <inheritdoc/>
        [JsonIgnore]
        public IReadOnlyCollection<byte> RawData { get; private set; }
        /// <summary>ID of the user that invoked the group action.</summary>
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
        /// <summary>Type of group action.</summary>
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

        /// <summary>Parses invoker ID and action type from raw data.</summary>
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

        /// <summary>Parses action type.</summary>
        /// <param name="type">String type of action.</param>
        /// <returns>Parsed action type.</returns>
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
                case "owner":
                    return GroupActionType.OwnerChanged;
                default:
                    throw new ArgumentException($"Unknown group action type: {type}", nameof(type));
            }
        }
    }

    /// <summary>Group action type.</summary>
    public enum GroupActionType
    {
        /// <summary>User joined the group.</summary>
        UserJoined,     // join
        /// <summary>User left the group.</summary>
        UserLeft,       // leave
        /// <summary>User was banned from the group.</summary>
        Ban,            // ban
        /// <summary>User was kicked from the group.</summary>
        Kick,           // 
        /// <summary>User was silenced in the group.</summary>
        Silence,        // silence
        /// <summary>User was reset in the group.</summary>
        Reset,          // reset
        /// <summary>User was modded the group.</summary>
        Mod,            // mod
        /// <summary>User was admined the group.</summary>
        Admin,          // admin
        /// <summary>User was set as group owner.</summary>
        OwnerChanged    // owner
    }
}
