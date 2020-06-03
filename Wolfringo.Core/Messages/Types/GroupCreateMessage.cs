using Newtonsoft.Json;
using System;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo.Messages
{
    [ResponseType(typeof(GroupEditResponse))]
    public class GroupCreateMessage : IWolfMessage, IGroupEditMessage
    {
        public string Command => MessageCommands.GroupCreate;

        // main props
        [JsonProperty("name")]
        public string Name { get; private set; }
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
        private GroupCreateMessage() { }

        public class Builder
        {
            public string Name { get; }
            public string Description { get; }
            public bool IsPeekable { get; set; }
            public bool IsExtendedAdminEnabled { get; set; }
            public bool IsDiscoverable { get; set; }
            public int? EntryReputationLevel { get; set; }
            public WolfLanguage? Language { get; set; }
            public string LongDescription { get; set; }

            public Builder(string name, string description)
            {
                if (string.IsNullOrWhiteSpace(name))
                    throw new ArgumentNullException(nameof(name));
                if (string.IsNullOrWhiteSpace(description))
                    throw new ArgumentNullException(nameof(description));
                this.Name = name;
                this.Description = description;
            }

            public GroupCreateMessage Build()
            {
                return new GroupCreateMessage()
                {
                    Name = this.Name,
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
