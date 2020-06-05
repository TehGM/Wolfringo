using System;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo
{
    /// <summary>Arguments for message sent events.</summary>
    [Serializable]
    public class WolfMessageSentEventArgs : WolfMessageEventArgs
    {
        /// <summary>Server's response.</summary>
        public IWolfResponse Response { get; }

        /// <summary>Creates a new instance of event arguments.</summary>
        /// <param name="message">Wolf message sent.</param>
        /// <param name="response">Server's response.</param>
        public WolfMessageSentEventArgs(IWolfMessage message, IWolfResponse response) : base(message)
        {
            this.Response = response;
        }
    }
}
