using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace TehGM.Wolfringo.Utilities.Internal
{
    /// <summary>An utility class for handling services that should be disposed (when created by provider/builder, and not by library user).</summary>
    /// <remarks>Do NOT register this service manually. It should only be initialized by builders, such as <see cref="WolfClientBuilder"/>.</remarks>
    public class DisposableServicesHandler : IDisposable
    {
        private readonly HashSet<Type> _markedForDisposal = new HashSet<Type>();
        private readonly List<IDisposable> _disposableServices = new List<IDisposable>();
        private readonly object _lock = new object();

        /// <summary>Marks service type to be disposed.</summary>
        /// <param name="type">Type of the service.</param>
        public void MarkForDisposal(Type type)
        {
            lock (this._lock)
                this._markedForDisposal.Add(type);
        }
        /// <summary>Marks service type to be disposed.</summary>
        /// <typeparam name="T">Type of the service.</typeparam>
        public void MarkForDisposal<T>()
            => this.MarkForDisposal(typeof(T));

        /// <summary>Removes disposal mark from the service type.</summary>
        /// <param name="type">Type of the service.</param>
        public void UnmarkForDisposal(Type type)
        {
            lock (this._lock)
                this._markedForDisposal.Remove(type);
        }
        /// <summary>Removes disposal mark from the service type.</summary>
        /// <typeparam name="T">Type of the service.</typeparam>
        public void UnmarkForDisposal<T>()
            => this.UnmarkForDisposal(typeof(T));

        /// <summary>Gets required service from provider. If service is marked for disposal, it'll automatically be tracked and disposed when <see cref="Dispose"/> is used.</summary>
        /// <typeparam name="T">Type of the service to get.</typeparam>
        /// <param name="services">Service provider to resolve the service from.</param>
        /// <returns>The resolved service.</returns>
        public T GetRequiredService<T>(IServiceProvider services)
        {
            T result = services.GetRequiredService<T>();
            this.TrackService<T>(result);
            return result;
        }

        /// <summary>Gets service from provider. If service is marked for disposal, it'll automatically be tracked and disposed when <see cref="Dispose"/> is used.</summary>
        /// <typeparam name="T">Type of the service to get.</typeparam>
        /// <param name="services">Service provider to resolve the service from.</param>
        /// <returns>The resolved service; null if not found.</returns>
        public T GetService<T>(IServiceProvider services)
        {
            T result = services.GetService<T>();
            this.TrackService<T>(result);
            return result;
        }

        private void TrackService<T>(T service)
        {
            if (service == null)
                return;

            lock (this._lock)
            {
                if (service is IDisposable disposable && this._markedForDisposal.Contains(typeof(T)) == true && !this._disposableServices.Contains(disposable))
                    this._disposableServices.Add(disposable);
            }
        }

        /// <summary>Disposes all tracked disposable services, and stops tracking them.</summary>
        public void Dispose()
        {
            lock (this._lock)
            {
                foreach (IDisposable disposable in this._disposableServices)
                    try { disposable?.Dispose(); } catch { }
                this._disposableServices.Clear();
            }
        }
    }
}
