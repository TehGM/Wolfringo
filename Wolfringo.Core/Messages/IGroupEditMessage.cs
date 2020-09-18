using Newtonsoft.Json;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo
{
    /// <summary>Represents a message that is used to edit a group.</summary>
    [ResponseType(typeof(GroupEditResponse))]
    public interface IGroupEditMessage : IWolfMessage
    {
        // main props
        /// <summary>Group short description.</summary>
        [JsonProperty("description")]
        string Description { get; }
        /// <summary>Is message history visible even if user is not in the group?</summary>
        [JsonProperty("peekable")]
        bool IsPeekable { get; }
        // extended props
        /// <summary>Is extended admin privilege enabled in this group?</summary>
        [JsonProperty("advancedAdmin")]
        bool IsExtendedAdminEnabled { get; }
        /// <summary>Is group publicly discoverable?</summary>
        [JsonProperty("discoverable")]
        bool IsDiscoverable { get; }
        /// <summary>Group's entry reputation level.</summary>
        /// <remarks>When this value is null, the group entry rep level setting will not change.</remarks>
        [JsonProperty("entryLevel")]
        int? EntryReputationLevel { get; }
        /// <summary>Language of the group.</summary>
        /// <remarks>When this value is null, the group language setting will not change.</remarks>
        [JsonProperty("language")]
        WolfLanguage? Language { get; }
        /// <summary>Long description of the group.</summary>
        /// <remarks>When this value is null, the group long description will not change.</remarks>
        [JsonProperty("longDescription")]
        string LongDescription { get; }
    }
}
