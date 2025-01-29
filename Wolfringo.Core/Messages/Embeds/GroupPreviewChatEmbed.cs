using Newtonsoft.Json;

namespace TehGM.Wolfringo.Messages.Embeds
{
    /// <summary>Represent a chat embed for group link.</summary>
    public class GroupPreviewChatEmbed : IChatEmbed
    {
        /// <inheritdoc/>
        public string EmbedType => "groupPreview";

        /// <summary>ID of the group to embed.</summary>
        [JsonProperty("groupId")]
        public uint GroupID { get; }

        /// <summary>Creates a new group preview embed with given group ID.</summary>
        /// <param name="groupID">ID of the group.</param>
        public GroupPreviewChatEmbed(uint groupID)
        {
            this.GroupID = groupID;
        }
    }
}
