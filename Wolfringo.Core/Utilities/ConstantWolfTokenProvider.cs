using System;
using System.Text.RegularExpressions;

namespace TehGM.Wolfringo.Utilities
{
    /// <summary>Provides a pre-defined not-changing token used by Wolf client when connecting.</summary>
    public class ConstantWolfTokenProvider : IWolfTokenProvider
    {
        /// <summary>The expected length of a token.</summary>
        public const int TokenLength = 18;
        private static readonly Regex _charsetRegex = new Regex("^[a-zA-Z0-9]+$", RegexOptions.CultureInvariant);

        /// <summary>The value of the token.</summary>
        public string Value { get; }

        /// <summary>Creates a token provider from a pre-defined token.</summary>
        /// <param name="token">Pre-defined token value.</param>
        public ConstantWolfTokenProvider(string token)
        {
            if (token == null)
                throw new ArgumentNullException(nameof(token));
            if (token.Length != 18)
                throw new ArgumentException($"Token must be {TokenLength} characters long.", nameof(token));
            if (!_charsetRegex.IsMatch(token))
                throw new ArgumentException("Token contains some invalid characters.", nameof(token));

            this.Value = token;
        }

        /// <inheritdoc/>
        /// <summary>Returns the token specified when creating the provider.</summary>
        public string GetToken()
            => this.Value;
    }
}
