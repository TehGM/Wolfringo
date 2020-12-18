using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace TehGM.Wolfringo.Commands.Initialization
{
    /// <summary>A service that loads Command instance descriptors from types and assemblies.</summary>
    public interface ICommandsLoader
    {
        /// <summary>Loads all command instance descriptors from an assembly.</summary>
        /// <param name="assembly">Assembly to load descriptors from.</param>
        /// <param name="cancellationToken">Token that can be used to stop loading.</param>
        /// <returns>Enumerable of all loaded command descriptors.</returns>
        Task<IEnumerable<ICommandInstanceDescriptor>> LoadFromAssemblyAsync(Assembly assembly, CancellationToken cancellationToken = default);
        /// <summary>Loads all command instance descriptors from a type.</summary>
        /// <param name="type">Type to load descriptors from.</param>
        /// <param name="cancellationToken">Token that can be used to stop loading.</param>
        /// <returns>Enumerable of all loaded command descriptors.</returns>
        Task<IEnumerable<ICommandInstanceDescriptor>> LoadFromTypeAsync(TypeInfo type, CancellationToken cancellationToken = default);
        /// <summary>Loads all command instance descriptors from a method.</summary>
        /// <param name="method">Method to load descriptors from.</param>
        /// <param name="cancellationToken">Token that can be used to stop loading.</param>
        /// <returns>Enumerable of all loaded command descriptors.</returns>
        Task<IEnumerable<ICommandInstanceDescriptor>> LoadFromMethodAsync(MethodInfo method, CancellationToken cancellationToken = default);
    }
}
