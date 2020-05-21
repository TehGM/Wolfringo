using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TehGM.Wolfringo.Socket
{
    /// <summary>Represents a basic SocketIO client to function with WolfClient.</summary>
    public interface ISocketClient
    {
        /// <summary>Is the client currently connected?</summary>
        bool IsConnected { get; }

        /// <summary>Rasied when the client connects to the server.</summary>
        event EventHandler Connected;
        /// <summary>Raised when the client disconnects from the server.</summary>
        event EventHandler<SocketClosedEventArgs> Disconnected;
        /// <summary>Raised when the client has received a message.</summary>
        event EventHandler<SocketMessageEventArgs> MessageReceived;
        /// <summary>Raised when the client has sent a message.</summary>
        event EventHandler<SocketMessageEventArgs> MessageSent;
        /// <summary>Raised when an error has occured in client's internal connection loop.</summary>
        event EventHandler<UnhandledExceptionEventArgs> ErrorRaised;

        /// <summary>Connect the client to the server.</summary>
        /// <param name="url">URL to connect to.</param>
        /// <param name="cancellationToken">Token which can be used to teminate the connection.</param>
        Task ConnectAsync(Uri url, CancellationToken cancellationToken = default);
        /// <summary>Disconnects the client from the server.</summary>
        /// <param name="cancellationToken">Token which can be used to terminate socket closing handshake.</param>
        Task DisconnectAsync(CancellationToken cancellationToken = default);
        /// <summary>Sends (emits) event to the server.</summary>
        /// <param name="payload">Json payload to send.</param>
        /// <param name="binaryMessages">Collection of binary messages to send. <paramref name="payload"/> should be pre-populated with placeholders.</param>
        /// <param name="cancellationToken">Token which can be used to abort sending.</param>
        /// <returns>ID of the sent message.</returns>
        Task<uint> SendAsync(JToken payload, IEnumerable<byte[]> binaryMessages, CancellationToken cancellationToken = default);
    }
}
