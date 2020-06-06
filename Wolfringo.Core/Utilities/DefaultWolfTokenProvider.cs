using System;
using System.Text;

namespace TehGM.Wolfringo.Utilities
{
    /// <inheritdoc/>
    public class DefaultWolfTokenProvider : ITokenProvider
    {
        private const string _charset = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPSADFGHJKLZXCVBNM1234567890";
        private static readonly Random _random = new Random();
        private const int _minLength = 2;

        /// <inheritdoc/>
        public string GenerateToken(int length)
        {
            if (length < _minLength)
                throw new ArgumentException($"Length of token can't be less than {_minLength}", nameof(length));

            StringBuilder builder = new StringBuilder(length);
            builder.Append("WE");
            for (int i = _minLength; i < length; i++)
                builder.Append(_charset[_random.Next(_charset.Length)]);
            return builder.ToString();
        }
    }
}
