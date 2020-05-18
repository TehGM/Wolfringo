using System;
using System.Threading;
using System.Threading.Tasks;

namespace TehGM.Wolfringo.Socket
{
    public interface ISocketClient
    {
        event EventHandler<SocketClosedEventArgs> Disconnected;

        Task ConnectAsync(Uri url, CancellationToken cancellationToken = default);
        Task DisconnectAsync(CancellationToken cancellationToken = default);
    }
}
