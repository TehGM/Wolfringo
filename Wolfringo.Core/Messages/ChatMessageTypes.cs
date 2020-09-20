namespace TehGM.Wolfringo.Messages
{
    /// <summary>Mime types of Wolf messages.</summary>
    public static class ChatMessageTypes
    {
        /// <summary>Text message.</summary>
        public const string Text = "text/plain";
        /// <summary>Received image message (as link).</summary>
        public const string ImageLink = "text/image_link";
        /// <summary>Sent image message.</summary>
        public const string Image = "image/jpeg";
        /// <summary>Received voice message (as link).</summary>
        public const string VoiceLink = "text/voice_link";
        /// <summary>Sent voice message.</summary>
        public const string Voice = "audio/x-speex";
        /// <summary>Group action.</summary>
        public const string GroupAction = "application/palringo-group-action";
        /// <summary>Response to private chat request.</summary>
        public const string PrivateRequestResponse = "text/palringo-private-request-response";
    }
}
