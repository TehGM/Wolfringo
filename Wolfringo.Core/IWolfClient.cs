using System;
using System.Threading;
using System.Threading.Tasks;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo
{
    public interface IWolfClient
    {
        WolfUser CurrentUser { get; }

        Task ConnectAsync(CancellationToken cancellationToken = default);
        Task DisconnectAsync(CancellationToken cancellationToken = default);
        Task<TResponse> SendAsync<TResponse>(IWolfMessage message, CancellationToken cancellationToken = default) where TResponse : WolfResponse;

        event Action<IWolfMessage> MessageReceived;
    }
}
