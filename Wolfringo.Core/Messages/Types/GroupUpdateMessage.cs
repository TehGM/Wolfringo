using Newtonsoft.Json;
using System;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo.Messages
{
    /// <summary>A message for updating a group.</summary>
    /// <remarks>Uses <see cref="GroupEditResponse"/> as response type.</remarks>
    [ResponseType(typeof(GroupEditResponse))]
    public class GroupUpdateMessage : IWolfMessage, IGroupEditMessage
    {
        /// <inheritdoc/>
        /// <remarks>Equals to <see cref="MessageEventNames.GroupProfileUpdate"/>.</remarks>
        [JsonIgnore]
        public string EventName => MessageEventNames.GroupProfileUpdate;

        // main props
        /// <summary>ID of the group.</summary>
        [JsonProperty("id")]
        public uint ID { get; protected set; }
        /// <summary>Group's short description.</summary>
        [JsonProperty("description")]
        public string Description { get; protected set; }
        /// <summary>Is message history visible even if user is not in the group?</summary>
        [JsonProperty("peekable")]
        public bool IsPeekable { get; protected set; }
        // extended props
        /// <summary>Is extended admin privilege enabled in this group?</summary>
        [JsonProperty("advancedAdmin")]
        public bool IsExtendedAdminEnabled { get; protected set; }
        /// <summary>Is group publicly discoverable?</summary>
        [JsonProperty("discoverable")]
        public bool IsDiscoverable { get; protected set; }
        /// <summary>Group's entry reputation level.</summary>
        [JsonProperty("entryLevel")]
        public int? EntryReputationLevel { get; protected set; }
        /// <summary>Language of the group.</summary>
        [JsonProperty("language")]
        public WolfLanguage? Language { get; protected set; }
        /// <summary>Long description of the group.</summary>
        [JsonProperty("longDescription")]
        public string LongDescription { get; protected set; }

        /// <summary>Creates a message instance.</summary>
        [JsonConstructor]
        protected GroupUpdateMessage() { }

        /// <summary>A builder class for <see cref="GroupUpdateMessage"/>.</summary>
        public class Builder
        {
            /// <summary>ID of the group.</summary>
            public uint ID { get; }
            /// <summary>Group's short description.</summary>
            public string Description { get; set; }
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

            /// <summary>Create a new builder for <see cref="GroupUpdateMessage"/>.</summary>
            /// <param name="group">Group to update.</param>
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

            /// <summary>Build the <see cref="GroupUpdateMessage"/>.</summary>
            /// <returns>A new <see cref="GroupUpdateMessage"/>.</returns>
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
