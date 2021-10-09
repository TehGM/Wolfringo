using System;
using TehGM.Wolfringo.Hosting;
using TehGM.Wolfringo.Messages.Serialization;
using TehGM.Wolfringo.Utilities;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <inheritdoc/>
    class HostedWolfClientServiceBuilder : IHostedWolfClientServiceBuilder
    {
        private readonly IServiceCollection _services;

        /// <summary>Creates a new instance of the builder.</summary>
        /// <param name="services">Service collection.</param>
        /// <exception cref="ArgumentNullException">Service collection is null.</exception>
        public HostedWolfClientServiceBuilder(IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            this._services = services;
        }

        /// <inheritdoc/>
        public IHostedWolfClientServiceBuilder Configure(Action<HostedWolfClientOptions> configure)
        {
            this._services.Configure<HostedWolfClientOptions>(configure);
            return this;
        }

        /// <inheritdoc/>
        public IHostedWolfClientServiceBuilder ConfigureCaching(Action<WolfCacheOptions> configure)
        {
            this._services.Configure<WolfCacheOptions>(configure);
            return this;
        }

        /// <inheritdoc/>
        public IHostedWolfClientServiceBuilder ConfigureMessageSerializerProvider(Action<MessageSerializerProviderOptions> configure)
        {
            this._services.Configure<MessageSerializerProviderOptions>(configure);
            return this;
        }

        /// <inheritdoc/>
        public IHostedWolfClientServiceBuilder ConfigureResponseSerializerProvider(Action<ResponseSerializerProviderOptions> configure)
        {
            this._services.Configure<ResponseSerializerProviderOptions>(configure);
            return this;
        }
    }
}
