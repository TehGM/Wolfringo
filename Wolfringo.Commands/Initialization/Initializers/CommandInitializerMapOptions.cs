using System;
using System.Collections.Generic;

namespace TehGM.Wolfringo.Commands.Initialization
{
    public class CommandInitializerMapOptions
    {
        public IDictionary<Type, ICommandInitializer> Initializers { get; set; } = new Dictionary<Type, ICommandInitializer>()
        {
            { typeof(RegexCommandAttribute), new RegexCommandInitializer() },
            { typeof(CommandAttribute), new StandardCommandInitializer() }
        };
    }
}
