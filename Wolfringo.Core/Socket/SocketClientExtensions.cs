using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TehGM.Wolfringo.Socket
{
    public static class SocketClientExtensions
    {
        public static Task<uint> SendAsync(this ISocketClient client, string eventName, JToken data, IEnumerable<byte[]> binaryMessages, CancellationToken cancellationToken = default)
            => client.SendAsync(new JArray(eventName, data), binaryMessages, cancellationToken);
    }
}
