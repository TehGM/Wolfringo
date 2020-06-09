using System.Threading;
using System.Threading.Tasks;

namespace TehGM.Wolfringo.Utilities
{
    /// <summary>Listener allowing to await next message.</summary>
    /// <typeparam name="T">Type of message</typeparam>
    public interface IInteractiveListener<T> where T : IWolfMessage
    {
        /// <summary>Awaits next message of given type.</summary>
        /// <param name="client">Client to await message from.</param>
        /// <returns>Next message.</returns>
        Task<T> AwaitNextAsync(IWolfClient client, CancellationToken cancellationToken = default);
    }
}
