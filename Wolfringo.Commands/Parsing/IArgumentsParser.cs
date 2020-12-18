using System.Collections.Generic;

namespace TehGM.Wolfringo.Commands.Parsing
{
    /// <summary>An argument parser for commands.</summary>
    public interface IArgumentsParser
    {
        /// <summary>Parses commands from input.</summary>
        /// <param name="input">Command input.</param>
        /// <param name="startIndex">Index at which to start parsing.</param>
        /// <returns>Enumerable of found arguments.</returns>
        IEnumerable<string> ParseArguments(string input, int startIndex);
    }
}
