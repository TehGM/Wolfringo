using System;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo
{
    [Serializable]
    public class WolfMessageSentEventArgs : WolfMessageEventArgs
    {
        public IWolfResponse Response { get; }

        public WolfMessageSentEventArgs(IWolfMessage message, IWolfResponse response) : base(message)
        {
            this.Response = response;
        }
    }
}
