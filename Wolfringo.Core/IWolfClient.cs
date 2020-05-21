using System;
using System.Threading.Tasks;

namespace TehGM.Wolfringo
{
    public interface IWolfClient
    {
        Task ConnectAsync();
        Task DisconnectAsync();
        Task<TResponse> SendAsync<TResponse>(IWolfMessage message) where TResponse : class;

        event Action<IWolfMessage> MessageReceived;
    }
}
