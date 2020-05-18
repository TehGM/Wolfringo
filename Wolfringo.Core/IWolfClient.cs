using System;
using System.Threading.Tasks;

namespace TehGM.Wolfringo
{
    public interface IWolfClient
    {
        Task ConnectAsync();
        Task DisconnectAsync();
        Task SendAsync(IWolfMessage message);

        event Action<IWolfMessage> MessageReceived;
    }
}
