using Newtonsoft.Json;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo
{
    [ResponseType(typeof(GroupEditResponse))]
    public interface IGroupEditMessage : IWolfMessage
    {
        // main props
        [JsonProperty("description")]
        string Description { get; }
        [JsonProperty("peekable")]
        bool IsPeekable { get; }
        // extended props
        [JsonProperty("advancedAdmin")]
        bool IsExtendedAdminEnabled { get; }
        [JsonProperty("discoverable")]
        bool IsDiscoverable { get; }
        [JsonProperty("entryLevel")]
        int? EntryReputationLevel { get; }
        [JsonProperty("language")]
        WolfLanguage? Language { get; }
        [JsonProperty("longDescription")]
        string LongDescription { get; }
    }
}
