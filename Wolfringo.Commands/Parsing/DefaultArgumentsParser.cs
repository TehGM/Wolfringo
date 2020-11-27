using System.Collections.Generic;
using System.Linq;

namespace TehGM.Wolfringo.Commands.Parsing
{
    /// <inheritdoc/>
    public class DefaultArgumentsParser : IArgumentsParser
    {
        private DefaultArgumentsParserOptions _options;

        /// <summary>Initial size allocated for an argument.</summary>
        /// <remarks><para><see cref="DefaultArgumentsParser"/> uses a new list of <see cref="char"/> for each argument, and allocates initial size.
        /// A good initial size is big enough to contain most commonly used arguments, but small enough to not allocate too much memory unnecessarily.</para>
        /// <para>Defaults to 8.</para></remarks>
        public int InitialBlockSizeAllocation => _options.InitialBlockSizeAllocation;
        /// <summary>Base marker to use by default.</summary>
        /// <remarks><para>Base marker is used by default, and will be skipped inside of a nested block.</para>
        /// <para>Base marker must be contained as a key in <see cref="BlockMarkers"/>.</para>
        /// <para>Defaults to space.</para></remarks>
        public char BaseMarker => _options.BaseMarker;
        private char _baseTerminator => this.BlockMarkers[this.BaseMarker];
        /// <summary>Argument start and end markers.</summary>
        /// <remarks>By default, following markers are used to split arguments:<br/>
        /// Start: ' ', End: ' '<br/>
        /// Start: '"', End: '"'<br/>
        /// Start: '(', End: ')'<br/>
        /// Start: '[', End: ']'<br/></remarks>
        public IDictionary<char, char> BlockMarkers => _options.BlockMarkers;

        /// <summary>Create a new instance of default parser.</summary>
        /// <param name="options">Options to use with this parser.</param>
        public DefaultArgumentsParser(DefaultArgumentsParserOptions options)
        {
            this._options = options;
        }

        /// <summary>Create a new instance of default parser using default options.</summary>
        public DefaultArgumentsParser() : this(new DefaultArgumentsParserOptions()) { }

        /// <inheritdoc/>
        public IEnumerable<string> ParseArguments(string input, int startIndex)
        {
            ICollection<string> results = new List<string>();

            int cursor = startIndex;
            while (cursor < input.Length)
                ParseBlock(input, ref cursor, _baseTerminator, ref results);

            return results.Where(s => !string.IsNullOrWhiteSpace(s));
        }

        private void ParseBlock(string input, ref int cursor, char? terminator, ref ICollection<string> results)
        {
            List<char> block = new List<char>(this.InitialBlockSizeAllocation);

            while (cursor < input.Length)
            {
                // get next character
                char character = input[cursor];
                cursor++;   // move cursor early - so sub or parent block start with a next character
                // if it's a start of a new block, parse it as a sub block with terminator, but only if we're in a base block
                // terminate current block as well - nested blocks are not allowed, to avoid unnecessary complexity
                if (terminator == _baseTerminator && character != this.BaseMarker && this.BlockMarkers.TryGetValue(character, out char subBlockTerminator))
                {
                    results.Add(new string(block.ToArray()));   // to maintain order
                    ParseBlock(input, ref cursor, subBlockTerminator, ref results);
                    return;
                }
                // if character is not a terminator, add to result
                else if (character != terminator)
                    block.Add(character);
                // if it is a terminator, terminate block
                else
                    break;
            }
            if (block.Count != 0)
                results.Add(new string(block.ToArray()));
        }
    }
}
