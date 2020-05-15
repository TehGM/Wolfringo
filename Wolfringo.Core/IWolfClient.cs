using System;
using System.Threading.Tasks;

namespace TehGM.Wolfringo
{
    public interface IWolfClient
    {
        Task ConnectAsync();
        Task DisconnectAsync();

        event Action Connected;
        event Action Disconnected;
    }
}
