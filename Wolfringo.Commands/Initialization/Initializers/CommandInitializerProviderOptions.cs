using System;
using System.Collections.Generic;

namespace TehGM.Wolfringo.Commands.Initialization
{
    /// <summary>Options for default command initializer provider.</summary>
    /// <seealso cref="CommandInitializerProvider"/>
    /// <see cref="ICommandInitializer"/>
    public class CommandInitializerProviderOptions
    {
        /// <summary>Map for command type and assigned command initializer.</summary>
        /// <remarks><para>Initializers mapped by default:<br/>
        /// <see cref="RegexCommandAttribute"/> - <see cref="RegexCommandInitializer"/><br/>
        /// <see cref="CommandAttribute"/> - <see cref="StandardCommandInitializer"/></para></remarks>
        public IDictionary<Type, ICommandInitializer> Initializers { get; set; } = new Dictionary<Type, ICommandInitializer>()
        {
            { typeof(RegexCommandAttribute), new RegexCommandInitializer() },
            { typeof(CommandAttribute), new StandardCommandInitializer() }
        };
    }
}
