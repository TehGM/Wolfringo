using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;
using TehGM.Wolfringo;
using TehGM.Wolfringo.Hosting;
using TehGM.Wolfringo.Messages.Responses;
using TehGM.Wolfringo.Messages.Serialization;
using TehGM.Wolfringo.Utilities;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWolfClient(this IServiceCollection services, Action<HostedWolfClientOptions> configureOptions = null)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.TryAddTransient<ITokenProvider, DefaultWolfTokenProvider>();
            services.TryAddTransient<IResponseTypeResolver, DefaultResponseTypeResolver>();
            services.TryAddTransient<ISerializerMap<string, IMessageSerializer>, DefaultMessageSerializerMap>();
            services.TryAddTransient<ISerializerMap<Type, IResponseSerializer>, DefaultResponseSerializerMap>();

            services.TryAddSingleton<IHostedWolfClient, HostedWolfClient>();
            services.TryAddSingleton<IWolfClient>(x => (IWolfClient)x.GetRequiredService<IHostedWolfClient>());
            services.AddTransient<IHostedService>(x => (IHostedService)x.GetRequiredService<IHostedWolfClient>());

            services.AddOptions();
            if (configureOptions != null)
                services.Configure(configureOptions);

            return services;
        }
    }
}
