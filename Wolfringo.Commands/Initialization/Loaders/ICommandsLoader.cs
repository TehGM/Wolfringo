using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace TehGM.Wolfringo.Commands.Initialization
{
    public interface ICommandsLoader
    {
        Task<IEnumerable<ICommandInstanceDescriptor>> LoadFromAssemblyAsync(Assembly assembly, CancellationToken cancellationToken = default);
        Task<IEnumerable<ICommandInstanceDescriptor>> LoadFromTypeAsync(TypeInfo type, CancellationToken cancellationToken = default);
        Task<IEnumerable<ICommandInstanceDescriptor>> LoadFromMethodAsync(MethodInfo method, CancellationToken cancellationToken = default);
    }
}
