using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TehGM.Wolfringo.Utilities.Internal
{
    /// <summary>Extension methods for <see cref="IServiceCollection"/> and <see cref="IServiceProvider"/> used by builder classes.</summary>
    public static class BuilderServicesExtensions
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

        /// <summary>Gets the most matching logger for a service.</summary>
        /// <typeparam name="TService">Service type.</typeparam>
        /// <typeparam name="TImplementation">Implementation type.</typeparam>
        /// <param name="services">Service provider to resolve logger from.</param>
        /// <returns>The most matching logger found; can still return null if none was found.</returns>
        public static ILogger GetLoggerFor<TService, TImplementation>(this IServiceProvider services) where TImplementation : TService
            => services.GetService<ILogger<TImplementation>>()
            ?? services.GetService<ILogger<TService>>()
            ?? services.GetService<ILoggerFactory>()?.CreateLogger<TImplementation>()
            ?? services.GetService<ILogger>();

        /// <summary>Gets the most matching logger for a service.</summary>
        /// <typeparam name="TService">Service type.</typeparam>
        /// <param name="services">Service provider to resolve logger from.</param>
        /// <returns>The most matching logger found; can still return null if none was found.</returns>
        public static ILogger GetLoggerFor<TService>(this IServiceProvider services)
            => GetLoggerFor<TService, TService>(services);
    }
}
