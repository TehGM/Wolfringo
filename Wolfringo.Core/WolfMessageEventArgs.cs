using System;

namespace TehGM.Wolfringo
{
    /// <summary>Arguments for wolf-message based events, such as message sent or received.</summary>
    [Serializable]
    public class WolfMessageEventArgs : EventArgs
    {
        /// <summary>Wolf message sent or received.</summary>
        public IWolfMessage Message { get; }

        /// <summary>Creates a new instance of event arguments.</summary>
        /// <param name="message">Wolf message sent or received.</param>
        public WolfMessageEventArgs(IWolfMessage message) : base()
        {
            this.Message = message;
        }
    }
}
