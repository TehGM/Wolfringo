using System;
using System.Text;

namespace TehGM.Wolfringo.Utilities
{
    /// <summary>Provides a randomized token used by Wolf client when connecting.</summary>
    public class RandomizedWolfTokenProvider : IWolfTokenProvider
    {
        private const string _charset = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPSADFGHJKLZXCVBNM1234567890";
        private static readonly Random _random = new Random();
        private const int _minLength = 2;

        /// <summary>Generate a new token with length of 18.</summary>
        /// <returns>Generated token.</returns>
        public string GetToken()
        {
            StringBuilder builder = new StringBuilder(ConstantWolfTokenProvider.TokenLength);
            builder.Append("WE");
            for (int i = _minLength; i < ConstantWolfTokenProvider.TokenLength; i++)
                builder.Append(_charset[_random.Next(_charset.Length)]);
            return builder.ToString();
        }
    }
}
