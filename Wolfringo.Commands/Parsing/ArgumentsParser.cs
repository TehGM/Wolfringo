using System.Collections.Generic;
using System.Linq;

namespace TehGM.Wolfringo.Commands.Parsing
{
    /// <inheritdoc/>
    public class ArgumentsParser : IArgumentsParser
    {
        /// <summary>Options used by this parser.</summary>
        protected ArgumentsParserOptions Options { get; }

        /// <summary>Initial size allocated for an argument.</summary>
        /// <remarks><para><see cref="ArgumentsParser"/> uses a new list of <see cref="char"/> for each argument, and allocates initial size.
        /// A good initial size is big enough to contain most commonly used arguments, but small enough to not allocate too much memory unnecessarily.</para>
        /// <para>Defaults to 8.</para></remarks>
        public int InitialBlockSizeAllocation => this.Options.InitialBlockSizeAllocation;
        /// <summary>Base marker to use by default.</summary>
        /// <remarks><para>Base marker is used by default, and will be skipped inside of a nested block.</para>
        /// <para>Base marker must be contained as a key in <see cref="BlockMarkers"/>.</para>
        /// <para>Defaults to space.</para></remarks>
        public char BaseMarker => this.Options.BaseMarker;
        private char _baseTerminator => this.BlockMarkers[this.BaseMarker];
        /// <summary>Argument start and end markers.</summary>
        /// <remarks>By default, following markers are used to split arguments:<br/>
        /// Start: ' ', End: ' '<br/>
        /// Start: '"', End: '"'<br/>
        /// Start: '(', End: ')'<br/>
        /// Start: '[', End: ']'<br/></remarks>
        public IDictionary<char, char> BlockMarkers => this.Options.BlockMarkers;

        /// <summary>Create a new instance of default parser.</summary>
        /// <param name="options">Options to use with this parser.</param>
        public ArgumentsParser(ArgumentsParserOptions options)
        {
            this.Options = options;
        }

        /// <summary>Create a new instance of default parser using default options.</summary>
        public ArgumentsParser() : this(new ArgumentsParserOptions()) { }

        /// <inheritdoc/>
        public IEnumerable<string> ParseArguments(string input, int startIndex)
        {
            ICollection<string> results = new List<string>();

            int cursor = startIndex;
            while (cursor < input.Length)
                this.ParseBlock(input, ref cursor, _baseTerminator, ref results);

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
                    this.ParseBlock(input, ref cursor, subBlockTerminator, ref results);
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
