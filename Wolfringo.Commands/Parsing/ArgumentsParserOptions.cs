using System.Collections.Generic;

namespace TehGM.Wolfringo.Commands.Parsing
{
    /// <summary>Options for default argument parser.</summary>
    /// <seealso cref="ArgumentsParser"/>
    public class ArgumentsParserOptions
    {
        /// <summary>Initial size allocated for an argument.</summary>
        /// <remarks><para><see cref="ArgumentsParser"/> uses a new list of <see cref="char"/> for each argument, and allocates initial size.
        /// A good initial size is big enough to contain most commonly used arguments, but small enough to not allocate too much memory unnecessarily.</para>
        /// <para>Defaults to 8.</para></remarks>
        public int InitialBlockSizeAllocation { get; set; } = 8;
        /// <summary>Base marker to use by default.</summary>
        /// <remarks><para>Base marker is used by default, and will be skipped inside of a nested block.</para>
        /// <para>Base marker must be contained as a key in <see cref="BlockMarkers"/>.</para>
        /// <para>Defaults to space.</para></remarks>
        public char BaseMarker { get; set; } = ' ';
        /// <summary>Argument start and end markers.</summary>
        /// <remarks>By default, following markers are used to split arguments:<br/>
        /// Start: ' ', End: ' '<br/>
        /// Start: '"', End: '"'<br/>
        /// Start: '(', End: ')'<br/>
        /// Start: '[', End: ']'<br/></remarks>
        public IDictionary<char, char> BlockMarkers { get; } = new Dictionary<char, char>
            {
                { ' ', ' ' },
                { '"', '"' },
                { '(', ')' },
                { '[', ']' }
            };
}
}
