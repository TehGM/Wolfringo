using Newtonsoft.Json;
using System;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>A message for creating a group.</summary>
    /// <remarks>Uses <see cref="GroupEditResponse"/> as response type.</remarks>
    [ResponseType(typeof(GroupEditResponse))]
    public class GroupCreateMessage : IWolfMessage, IGroupEditMessage
    {
        /// <inheritdoc/>
        [JsonIgnore]
        public string Command => MessageCommands.GroupCreate;

        // main props
        /// <summary>Group public name.</summary>
        [JsonProperty("name")]
        public string Name { get; private set; }
        /// <summary>Group's short description.</summary>
        [JsonProperty("description")]
        public string Description { get; private set; }
        /// <summary>Is message history visible even if user is not in the group?</summary>
        [JsonProperty("peekable")]
        public bool IsPeekable { get; private set; }
        // extended props
        /// <summary>Is extended admin privilege enabled in this group?</summary>
        [JsonProperty("advancedAdmin")]
        public bool IsExtendedAdminEnabled { get; private set; }
        /// <summary>Is group publicly discoverable?</summary>
        [JsonProperty("discoverable")]
        public bool IsDiscoverable { get; private set; }
        /// <summary>Group's entry reputation level.</summary>
        [JsonProperty("entryLevel")]
        public int? EntryReputationLevel { get; private set; }
        /// <summary>Language of the group.</summary>
        [JsonProperty("language")]
        public WolfLanguage? Language { get; private set; }
        /// <summary>Long description of the group.</summary>
        [JsonProperty("longDescription")]
        public string LongDescription { get; private set; }

        [JsonConstructor]
        private GroupCreateMessage() { }

        public class Builder
        {
            /// <summary>Group public name.</summary>
            public string Name { get; }
            /// <summary>Group's short description.</summary>
            public string Description { get; }
            /// <summary>Is message history visible even if user is not in the group?</summary>
            public bool IsPeekable { get; set; }
            /// <summary>Is extended admin privilege enabled in this group?</summary>
            public bool IsExtendedAdminEnabled { get; set; }
            /// <summary>Is group publicly discoverable?</summary>
            public bool IsDiscoverable { get; set; }
            /// <summary>Group's entry reputation level.</summary>
            public int? EntryReputationLevel { get; set; }
            /// <summary>Language of the group.</summary>
            public WolfLanguage? Language { get; set; }
            /// <summary>Long description of the group.</summary>
            public string LongDescription { get; set; }

            /// <summary>Create a new builder for <see cref="GroupCreateMessage"/>.</summary>
            /// <param name="name">Group public name.</param>
            /// <param name="description">Group's short description.</param>
            public Builder(string name, string description)
            {
                if (string.IsNullOrWhiteSpace(name))
                    throw new ArgumentNullException(nameof(name));
                if (string.IsNullOrWhiteSpace(description))
                    throw new ArgumentNullException(nameof(description));
                this.Name = name;
                this.Description = description;
            }

            /// <summary>Build the <see cref="GroupCreateMessage"/>.</summary>
            /// <returns>A new <see cref="GroupCreateMessage"/>.</returns>
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
