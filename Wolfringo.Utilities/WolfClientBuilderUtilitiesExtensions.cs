using System;
using Microsoft.Extensions.DependencyInjection;
using TehGM.Wolfringo.Utilities;
using TehGM.Wolfringo.Utilities.Internal;

namespace TehGM.Wolfringo
{
    /// <summary>Extension methods for <see cref="WolfClientBuilder"/> that enable easy adding of utilities.</summary>
    public static class WolfClientBuilderUtilitiesExtensions
    {
        /// <summary>Adds <see cref="WolfClientReconnector"/> together with <see cref="WolfClient"/>, and allows configuration.</summary>
        /// <param name="clientBuilder">The WOLF client builder.</param>
        /// <param name="configure">Delegate that can be used to configure commands.</param>
        /// <returns>Current WOLF Client builder instance.</returns>
        public static WolfClientBuilder WithAutoReconnection(this WolfClientBuilder clientBuilder, Action<ReconnectorConfig> configure)
        {
            // get config
            ReconnectorConfig config = new ReconnectorConfig();
            configure?.Invoke(config);

            Action<WolfClient, IServiceProvider> onBuilt = null;
            onBuilt = (client, services) =>
            {
                // use handler class to resolve, to enable automatic disposal
                DisposableServicesHandler handler = services.GetService<DisposableServicesHandler>();

                // resolve to start
                WolfClientReconnector result = handler?.GetRequiredService<WolfClientReconnector>(services) 
                        ?? services.GetRequiredService<WolfClientReconnector>();
            };

            clientBuilder.Built += onBuilt;

            // add to builder - use a factory delegate so it's automatically disposed
            return clientBuilder.WithService<WolfClientReconnector>(provider =>
            {
                if (config.Log == null)
                    config.Log = provider.GetLoggerFor<WolfClientReconnector>();
                return new WolfClientReconnector(provider.GetRequiredService<IWolfClient>(), config);
            });
        }
    }
}
