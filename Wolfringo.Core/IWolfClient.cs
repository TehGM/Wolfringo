using System;
using System.Threading.Tasks;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo
{
    public interface IWolfClient
    {
        WolfUser CurrentUser { get; }

        Task ConnectAsync();
        Task DisconnectAsync();
        Task<TResponse> SendAsync<TResponse>(IWolfMessage message) where TResponse : WolfResponse;

        event Action<IWolfMessage> MessageReceived;
    }
}
