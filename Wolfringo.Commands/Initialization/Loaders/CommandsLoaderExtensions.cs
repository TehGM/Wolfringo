using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace TehGM.Wolfringo.Commands.Initialization
{
    public static class CommandsLoaderExtensions
    {
        public static async Task<IEnumerable<ICommandInstanceDescriptor>> LoadFromAssembliesAsync(this ICommandsLoader loader, IEnumerable<Assembly> assemblies, CancellationToken cancellationToken = default)
        {
            List<ICommandInstanceDescriptor> results = new List<ICommandInstanceDescriptor>();
            foreach (Assembly asm in assemblies)
                results.AddRange(await loader.LoadFromAssemblyAsync(asm, cancellationToken).ConfigureAwait(false));
            return results;
        }
        public static async Task<IEnumerable<ICommandInstanceDescriptor>> LoadFromTypesAsync(this ICommandsLoader loader, IEnumerable<TypeInfo> types, CancellationToken cancellationToken = default)
        {
            List<ICommandInstanceDescriptor> results = new List<ICommandInstanceDescriptor>();
            foreach (TypeInfo type in types)
                results.AddRange(await loader.LoadFromTypeAsync(type, cancellationToken).ConfigureAwait(false));
            return results;
        }
        public static async Task<IEnumerable<ICommandInstanceDescriptor>> LoadFromMethodsAsync(this ICommandsLoader loader, IEnumerable<MethodInfo> methods, CancellationToken cancellationToken = default)
        {
            List<ICommandInstanceDescriptor> results = new List<ICommandInstanceDescriptor>();
            foreach (MethodInfo method in methods)
                results.AddRange(await loader.LoadFromMethodAsync(method, cancellationToken).ConfigureAwait(false));
            return results;
        }
    }
}
