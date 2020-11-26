using System.Collections.Generic;
using System.Linq;

namespace TehGM.Wolfringo.Commands.Parsing
{
    /// <inheritdoc/>
    public class DefaultArgumentsParser : IArgumentsParser
    {
        private const int _assumedBlockSize = 8;
        private char _baseMarker;
        private char _baseTerminator;

        /// <summary>Base marker to use by default.</summary>
        /// <remarks>Defaults to space.</remarks>
        /// <exception cref="KeyNotFoundException">Thrown when setting base marker that is not defined in <see cref="BlockMarkers"/>.</exception>
        public char BaseMarker
        {
            get => this._baseMarker;
            set
            {
                this._baseTerminator = this.BlockMarkers[value];
                this._baseMarker = value;
            }
        }
        /// <summary>Argument start and end markers.</summary>
        /// <remarks>By default, following markers are used to split arguments:<br/>
        /// Start: ' ', End: ' '<br/>
        /// Start: '"', End: '"'<br/>
        /// Start: '(', End: ')'<br/>
        /// Start: '[', End: ']'<br/></remarks>
        public IDictionary<char, char> BlockMarkers { get; }

        /// <summary>Create a new instance of default parser.</summary>
        public DefaultArgumentsParser()
        {
            this.BlockMarkers = new Dictionary<char, char>
            {
                { ' ', ' ' },
                { '"', '"' },
                { '(', ')' },
                { '[', ']' }
            };
            this.BaseMarker = ' ';
        }

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
            List<char> block = new List<char>(_assumedBlockSize);

            while (cursor < input.Length)
            {
                // get next character
                char character = input[cursor];
                cursor++;   // move cursor early - so sub or parent block start with a next character
                // if it's a start of a new block, parse it as a sub block with terminator, but only if we're in a base block
                // terminate current block as well - nested blocks are not allowed, to avoid unnecessary complexity
                if (terminator == _baseTerminator && character != _baseMarker && this.BlockMarkers.TryGetValue(character, out char subBlockTerminator))
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
