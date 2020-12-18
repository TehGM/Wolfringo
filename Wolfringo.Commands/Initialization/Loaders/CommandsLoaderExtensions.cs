using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace TehGM.Wolfringo.Commands.Initialization
{
    /// <summary>Extensions for a command loaders.</summary>
    public static class CommandsLoaderExtensions
    {
        /// <summary>Loads all command instance descriptors from assemblies.</summary>
        /// <param name="loader">The loader to perform loading with.</param>
        /// <param name="assemblies">Assemblies to load descriptors from.</param>
        /// <param name="cancellationToken">Token that can be used to stop loading.</param>
        /// <returns>Enumerable of all loaded command descriptors.</returns>
        public static async Task<IEnumerable<ICommandInstanceDescriptor>> LoadFromAssembliesAsync(this ICommandsLoader loader, IEnumerable<Assembly> assemblies, CancellationToken cancellationToken = default)
        {
            List<ICommandInstanceDescriptor> results = new List<ICommandInstanceDescriptor>();
            foreach (Assembly asm in assemblies)
                results.AddRange(await loader.LoadFromAssemblyAsync(asm, cancellationToken).ConfigureAwait(false));
            return results;
        }
        /// <summary>Loads all command instance descriptors from types.</summary>
        /// <param name="loader">The loader to perform loading with.</param>
        /// <param name="types">Types to load descriptors from.</param>
        /// <param name="cancellationToken">Token that can be used to stop loading.</param>
        /// <returns>Enumerable of all loaded command descriptors.</returns>
        public static async Task<IEnumerable<ICommandInstanceDescriptor>> LoadFromTypesAsync(this ICommandsLoader loader, IEnumerable<TypeInfo> types, CancellationToken cancellationToken = default)
        {
            List<ICommandInstanceDescriptor> results = new List<ICommandInstanceDescriptor>();
            foreach (TypeInfo type in types)
                results.AddRange(await loader.LoadFromTypeAsync(type, cancellationToken).ConfigureAwait(false));
            return results;
        }
        /// <summary>Loads all command instance descriptors from methods.</summary>
        /// <param name="loader">The loader to perform loading with.</param>
        /// <param name="methods">Methods to load descriptors from.</param>
        /// <param name="cancellationToken">Token that can be used to stop loading.</param>
        /// <returns>Enumerable of all loaded command descriptors.</returns>
        public static async Task<IEnumerable<ICommandInstanceDescriptor>> LoadFromMethodsAsync(this ICommandsLoader loader, IEnumerable<MethodInfo> methods, CancellationToken cancellationToken = default)
        {
            List<ICommandInstanceDescriptor> results = new List<ICommandInstanceDescriptor>();
            foreach (MethodInfo method in methods)
                results.AddRange(await loader.LoadFromMethodAsync(method, cancellationToken).ConfigureAwait(false));
            return results;
        }
    }
}
