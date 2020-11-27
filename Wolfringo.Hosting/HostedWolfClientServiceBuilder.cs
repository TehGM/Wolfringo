using System;
using TehGM.Wolfringo.Hosting;
using TehGM.Wolfringo.Messages.Serialization;

namespace Microsoft.Extensions.DependencyInjection
{
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

        public IHostedWolfClientServiceBuilder Configure(Action<HostedWolfClient> configure)
        {
            this._services.Configure<HostedWolfClient>(configure);
            return this;
        }

        public IHostedWolfClientServiceBuilder ConfigureMessageSerializerProvider(Action<MessageSerializerProviderOptions> configure)
        {
            this._services.Configure<MessageSerializerProviderOptions>(configure);
            return this;
        }

        public IHostedWolfClientServiceBuilder ConfigureResponseSerializerProvider(Action<ResponseSerializerProviderOptions> configure)
        {
            this._services.Configure<ResponseSerializerProviderOptions>(configure);
            return this;
        }
    }
}
