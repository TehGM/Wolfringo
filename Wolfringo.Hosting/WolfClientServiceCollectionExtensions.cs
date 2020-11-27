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
    public static class WolfClientServiceCollectionExtensions
    {
        /// <summary>Adds Hosted Wolf Client to services as a hosted service, and all related services.</summary>
        /// <remarks><para>This method will also add <see cref="ITokenProvider"/>, <see cref="IResponseTypeResolver"/>,
        /// and <see cref="ISerializerProvider{TKey, TSerializer}"/> for messages and responses, unless already added.</para>
        /// <para>Added client will be injectable as both <see cref="IHostedWolfClient"/> and <see cref="IWolfClient"/>.</para></remarks>
        /// <param name="configureOptions">Configuration of client options.</param>
        public static IServiceCollection AddWolfClient(this IServiceCollection services, Action<HostedWolfClientOptions> configureOptions = null)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.TryAddTransient<ITokenProvider, DefaultWolfTokenProvider>();
            services.TryAddTransient<IResponseTypeResolver, DefaultResponseTypeResolver>();
            services.TryAddTransient<ISerializerProvider<string, IMessageSerializer>, DefaultMessageSerializerProvider>();
            services.TryAddTransient<ISerializerProvider<Type, IResponseSerializer>, DefaultResponseSerializerProvider>();

            services.TryAddSingleton<IWolfClient, HostedWolfClient>();
            services.AddTransient<IHostedService>(x => (IHostedService)x.GetRequiredService<IWolfClient>());

            services.AddOptions();
            if (configureOptions != null)
                services.Configure(configureOptions);

            return services;
        }
    }
}
