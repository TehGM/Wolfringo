using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TehGM.Wolfringo.Socket
{
    public static class SocketClientExtensions
    {
        /// <summary>Sends (emits) event to the server.</summary>
        /// <param name="eventName">Event name to send.</param>
        /// <param name="data">Json data to pack with <paramref name="eventName"/>.</param>
        /// <param name="binaryMessages">Collection of binary messages to send. <paramref name="payload"/> should be pre-populated with placeholders.</param>
        /// <param name="cancellationToken">Token which can be used to abort sending.</param>
        /// <returns>ID of the sent message.</returns>
        public static Task<uint> SendAsync(this ISocketClient client, string eventName, JToken data, IEnumerable<byte[]> binaryMessages, CancellationToken cancellationToken = default)
            => client.SendAsync(new JArray(eventName, data), binaryMessages, cancellationToken);
    }
}
