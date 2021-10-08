using System;
using Microsoft.Extensions.DependencyInjection;
using TehGM.Wolfringo.Commands;

namespace TehGM.Wolfringo
{
    /// <summary>Extension methods for <see cref="WolfClientBuilder"/> that enable easy creation together with commands.</summary>
    public static class WolfClientBuilderCommandsExtensions
    {
        /// <summary>Adds <see cref="CommandsService"/> together with <see cref="WolfClient"/>, and allows configuration.</summary>
        /// <remarks>When using this method, any calls to <see cref="CommandsServiceBuilder.WithWolfClient{TImplementation}"/> will be overriden.</remarks>
        /// <param name="clientBuilder">The WOLF client builder.</param>
        /// <param name="commandsBuilder">Delegate that can be used to configure commands.</param>
        /// <returns>Current WOLF Client builder instance.</returns>
        public static WolfClientBuilder WithCommands(this WolfClientBuilder clientBuilder, Action<CommandsServiceBuilder> commandsBuilder)
        {
            CommandsServiceBuilder builder = null;
            Action<IServiceCollection> onBuilding = services =>
            {
                builder = new CommandsServiceBuilder(services);
                commandsBuilder?.Invoke(builder);
            };
            Action<WolfClient> onBuilt = null;
            onBuilt = client =>
            {
                // add the related Wolf Client
                builder.WithWolfClient(client);

                // build and start
                CommandsService commands = builder.Build();
                commands.StartAsync(builder.Options.CancellationToken).GetAwaiter().GetResult();

                // remove event handlers
                clientBuilder.Building -= onBuilding;
                clientBuilder.Built -= onBuilt;
            };

            clientBuilder.Building += onBuilding;
            clientBuilder.Built += onBuilt;

            return clientBuilder;
        }
    }
}
