using System;
using System.Collections.Generic;
using TehGM.Wolfringo.Messages;

namespace TehGM.Wolfringo.Utilities.Internal
{
    /// <summary>Internal utility helper for detecting and building URL links in outgoing messages.</summary>
    public static class UrlLinkDetectionHelper
    {
        private static readonly char[] _endMarkers = new char[] { ' ', '\n', '\r' };

        /// <summary>Finds URL links in the text, and builds metadata for each found link.</summary>
        /// <param name="text">Text to find group links in.</param>
        public static IEnumerable<ChatMessageFormatting.LinkData> FindLinks(string text)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                int openIndex = FindIndexOfLink(text);
                while (openIndex > -1)
                {
                    int closeIndex = text.IndexOfAny(_endMarkers, openIndex);
                    if (closeIndex < 0)
                        closeIndex = text.Length;
                    int length = closeIndex - openIndex;
                    string url = text.Substring(openIndex, length);
                    if (!string.IsNullOrWhiteSpace(url))
                    {
                        url = url.TrimEnd('.');
                        yield return new ChatMessageFormatting.LinkData((uint)openIndex, (uint)(openIndex + url.Length), url);
                    }

                    openIndex = FindIndexOfLink(text, closeIndex);
                }
            }
        }

        private static int FindIndexOfLink(string text, int startIndex = 0)
        {
            int indexOfHttps = text.IndexOf("https://", startIndex, StringComparison.OrdinalIgnoreCase);
            int indexOfHttp = text.IndexOf("http://", startIndex, StringComparison.OrdinalIgnoreCase);
            if (indexOfHttp == -1)
                return indexOfHttps;
            if (indexOfHttps == -1)
                return indexOfHttp;
            return Math.Min(indexOfHttps, indexOfHttp);
        }
    }
}
