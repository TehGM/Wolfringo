using System;
using System.Collections.Generic;

namespace TehGM.Wolfringo.Commands.Initialization
{
    /// <summary>A simple service provider.</summary>
    /// <remarks>This service provider is used automatically by <see cref="CommandsService"/> when no own <see cref="IServiceProvider"/> is provided via constructor injection.</remarks>
    public class SimpleServiceProvider : IServiceProvider
    {
        private IDictionary<Type, object> _services;

        /// <summary>Creates a new instance of service provider.</summary>
        /// <param name="services">Services mapping.</param>
        public SimpleServiceProvider(IDictionary<Type, object> services)
        {
            this._services = services ?? new Dictionary<Type, object>();

            if (!_services.TryGetValue(typeof(IServiceProvider), out _))
                _services.Add(typeof(IServiceProvider), this);
            if (!_services.TryGetValue(this.GetType(), out _))
                _services.Add(this.GetType(), this);
        }

        /// <inheritdoc/>
        public object GetService(Type serviceType)
        {
            lock (_services)
            {
                this._services.TryGetValue(serviceType, out object result);
                return result;
            }
        }
    }
}
