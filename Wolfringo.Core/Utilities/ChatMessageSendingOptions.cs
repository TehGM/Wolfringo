namespace TehGM.Wolfringo.Utilities
{
    /// <summary>Configuration for behaviour of sending utilities when sending a chat message.</summary>
    public sealed class ChatMessageSendingOptions
    {
        /// <summary>Default options.</summary>
        /// <remarks>These options will automatically detect everything - group and website links, and enable all preview embeds.</remarks>
        public static ChatMessageSendingOptions Default { get; } = new ChatMessageSendingOptions();

#if NET5_0_OR_GREATER
        /// <summary>Whether group links should be automatically detected.</summary>
        public bool AutoDetectGroupLinks { get; init; }
        /// <summary>Whether website links should be automatically detected.</summary>
        public bool AutoDetectWebsiteLinks { get; init; }
        /// <summary>Whether group preview should be displayed as embed.</summary>
        /// <remarks>Doesn't have any effect if <see cref="AutoDetectGroupLinks"/> is set to false.</remarks>
        public bool EnableGroupLinkPreview { get; init; }
        /// <summary>Whether link preview should be displayed as embed.</summary>
        /// <remarks>Doesn't have any effect if <see cref="AutoDetectWebsiteLinks"/> is set to false.</remarks>
        public bool EnableWebsiteLinkPreview { get; init; }
        /// <summary>Whether image preview should be displayed as embed.</summary>
        /// <remarks>Doesn't have any effect if <see cref="AutoDetectWebsiteLinks"/> is set to false.</remarks>
        public bool EnableImageLinkPreview { get; init; }
#else
        /// <summary>Whether group links should be automatically detected.</summary>
        public bool AutoDetectGroupLinks { get; set; }
        /// <summary>Whether website links should be automatically detected.</summary>
        public bool AutoDetectWebsiteLinks { get; set; }
        /// <summary>Whether group preview should be displayed as embed.</summary>
        /// <remarks>Doesn't have any effect if <see cref="AutoDetectGroupLinks"/> is set to false.</remarks>
        public bool EnableGroupLinkPreview { get; set; }
        /// <summary>Whether website preview should be displayed as embed.</summary>
        /// <remarks>Doesn't have any effect if <see cref="AutoDetectWebsiteLinks"/> is set to false.</remarks>
        public bool EnableWebsiteLinkPreview { get; set; }
        /// <summary>Whether image preview should be displayed as embed.</summary>
        /// <remarks>Doesn't have any effect if <see cref="AutoDetectWebsiteLinks"/> is set to false.</remarks>
        public bool EnableImageLinkPreview { get; set; }
#endif

        /// <summary>Creates a new instance of options, with all flags set to true.</summary>
        public ChatMessageSendingOptions()
        {
            this.AutoDetectGroupLinks = true;
            this.AutoDetectWebsiteLinks = true;
            this.EnableGroupLinkPreview = true;
            this.EnableWebsiteLinkPreview = true;
            this.EnableImageLinkPreview = true;
        }
    }
}
