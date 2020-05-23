using System;

namespace TehGM.Wolfringo
{
    [Serializable]
    public class WolfMessageEventArgs : EventArgs
    {
        public IWolfMessage Message { get; }

        public WolfMessageEventArgs(IWolfMessage message)
        {
            this.Message = message;
        }
    }
}
