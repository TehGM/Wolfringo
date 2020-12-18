using System;
using System.Collections.Generic;
using System.Linq;

namespace TehGM.Wolfringo.Commands.Initialization
{
    /// <summary>Utility service provider, that allows using multiple service providers as one.</summary>
    public class CombinedServiceProvider : IServiceProvider
    {
        private IEnumerable<IServiceProvider> _providers;

        /// <summary>Creates a new combined service provider instance.</summary>
        /// <param name="serviceProviders">Service providers to combine.</param>
        public CombinedServiceProvider(IEnumerable<IServiceProvider> serviceProviders)
        {
            this._providers = serviceProviders?.Where(p => p != null) ?? Enumerable.Empty<IServiceProvider>();
        }

        /// <inheritdoc/>
        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IServiceProvider))
                return this;

            foreach (IServiceProvider provider in this._providers)
            {
                object result = provider.GetService(serviceType);
                if (result != null)
                    return result;
            }

            if (serviceType.IsAssignableFrom(this.GetType()))
                return this;
            return null;
        }

        /// <summary>Combines multiple service providers.</summary>
        /// <param name="serviceProviders">Service providers to combine.</param>
        public static CombinedServiceProvider Combine(params IServiceProvider[] serviceProviders)
            => new CombinedServiceProvider(serviceProviders);
    }
}
