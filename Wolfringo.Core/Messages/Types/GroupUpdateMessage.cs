using Newtonsoft.Json;
using System;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo.Messages
{
    [ResponseType(typeof(GroupEditResponse))]
    public class GroupUpdateMessage : IWolfMessage, IGroupEditMessage
    {
        public string Command => MessageCommands.GroupProfileUpdate;

        // main props
        [JsonProperty("id")]
        public uint ID { get; private set; }
        [JsonProperty("description")]
        public string Description { get; private set; }
        [JsonProperty("peekable")]
        public bool IsPeekable { get; private set; }
        // extended props
        [JsonProperty("advancedAdmin")]
        public bool IsExtendedAdminEnabled { get; private set; }
        [JsonProperty("discoverable")]
        public bool IsDiscoverable { get; private set; }
        [JsonProperty("entryLevel")]
        public int? EntryReputationLevel { get; private set; }
        [JsonProperty("language")]
        public WolfLanguage? Language { get; private set; }
        [JsonProperty("longDescription")]
        public string LongDescription { get; private set; }

        [JsonConstructor]
        private GroupUpdateMessage() { }

        public class Builder
        {
            public uint ID { get; }
            public string Description { get; set; }
            public bool IsPeekable { get; set; }
            public bool IsExtendedAdminEnabled { get; set; }
            public bool IsDiscoverable { get; set; }
            public int? EntryReputationLevel { get; set; }
            public WolfLanguage? Language { get; set; }
            public string LongDescription { get; set; }

            public Builder(WolfGroup group)
            {
                if (group == null)
                    throw new ArgumentNullException(nameof(group));

                this.ID = group.ID;
                this.Description = group.Description;
                this.IsPeekable = group.IsPeekable;
                this.IsExtendedAdminEnabled = group.IsExtendedAdminEnabled ?? false;
                this.IsDiscoverable = group.IsDiscoverable ?? false;
                this.EntryReputationLevel = group.EntryReputationLevel;
                this.Language = group.Language;
                this.LongDescription = group.LongDescription;
            }

            public GroupUpdateMessage Build()
            {
                return new GroupUpdateMessage()
                {
                    ID = this.ID,
                    Description = this.Description,
                    IsPeekable = this.IsPeekable,
                    IsExtendedAdminEnabled = this.IsExtendedAdminEnabled,
                    IsDiscoverable = this.IsDiscoverable,
                    EntryReputationLevel = this.EntryReputationLevel,
                    Language = this.Language,
                    LongDescription = this.LongDescription
                };
            }
        }
    }
}
