using System;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo
{
    [Serializable]
    public class WolfMessageSentEventArgs : WolfMessageEventArgs
    {
        public WolfResponse Response { get; }

        public WolfMessageSentEventArgs(IWolfMessage message, WolfResponse response) : base(message)
        {
            this.Response = response;
        }
    }
}
