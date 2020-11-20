using System;
using System.Collections.Generic;

namespace TehGM.Wolfringo.Commands.Initialization
{
    public class SimpleServiceProvider : IServiceProvider
    {
        private IDictionary<Type, object> _services;

        public SimpleServiceProvider(IDictionary<Type, object> services)
        {
            this._services = services ?? new Dictionary<Type, object>();

            if (!_services.TryGetValue(typeof(IServiceProvider), out _))
                _services.Add(typeof(IServiceProvider), this);
            if (!_services.TryGetValue(this.GetType(), out _))
                _services.Add(this.GetType(), this);
        }

        public object GetService(Type serviceType)
        {
            this._services.TryGetValue(serviceType, out object result);
            return result;
        }
    }
}
