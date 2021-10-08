using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace TehGM.Wolfringo.Utilities.Internal
{
    /// <summary>Extension methods for <see cref="IServiceCollection"/> used by builder classes.</summary>
    public static class BuilderServiceCollectionExtensions
    {
        /// <summary>Removes a service of given type.</summary>
        /// <typeparam name="TService">Service type.</typeparam>
        /// <param name="services">Service collection to remove service from.</param>
        public static void RemoveService<TService>(this IServiceCollection services)
        {
            if (services.TryGetDescriptor<TService>(out ServiceDescriptor descriptor))
                services.Remove(descriptor);
        }

        /// <summary>Tries to get <see cref="ServiceDescriptor"/> for service of given type.</summary>
        /// <typeparam name="TService">Service type.</typeparam>
        /// <param name="services">Service collection to search in.</param>
        /// <param name="result">The found service descriptor.</param>
        /// <returns>True if service was found; otherwise false.</returns>
        public static bool TryGetDescriptor<TService>(this IServiceCollection services, out ServiceDescriptor result)
        {
            result = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(TService));
            return result != null;
        }

        /// <summary>Checks if service of given type is added to service collection.</summary>
        /// <typeparam name="TService">Service type.</typeparam>
        /// <param name="services">Service collection to search in.</param>
        /// <returns>True if service was found; otherwise false.</returns>
        public static bool HasService<TService>(this IServiceCollection services)
            => services.TryGetDescriptor<TService>(out _);
    }
}
