using System;
using System.Collections.Generic;
using System.Text;

namespace TehGM.Wolfringo.Messages
{
    public class WelcomeMessage : IWolfMessage
    {
        public string Command => MessageCommands.Welcome;
    }
}
