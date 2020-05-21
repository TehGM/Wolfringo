namespace TehGM.Wolfringo.Messages
{
    public static class ChatMessageTypes
    {
        public const string Text = "text/plain";
        public const string ImageLink = "text/image_link";
        public const string Image = "image/jpeg";
        public const string VoiceLink = "text/voice_link";
        public const string Voice = "audio/x-speex";
        public const string GroupAction = "application/palringo-group-action";
        public const string PrivateRequestResponse = "text/palringo-private-request-response";
    }
}
