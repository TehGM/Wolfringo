using System.Threading.Tasks;
using TehGM.Wolfringo.Messages.Responses;

namespace TehGM.Wolfringo
{
    public static class WolfClientExtensions
    {
        public static Task<WolfResponse> SendAsync(this IWolfClient client, IWolfMessage message)
            => client.SendAsync<WolfResponse>(message);
    }
}
