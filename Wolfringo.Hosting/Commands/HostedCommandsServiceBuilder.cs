using System;
using Microsoft.Extensions.DependencyInjection;
using TehGM.Wolfringo.Commands;
using TehGM.Wolfringo.Commands.Initialization;
using TehGM.Wolfringo.Commands.Parsing;

namespace TehGM.Wolfringo.Hosting.Commands
{
    /// <inheritdoc/>
    public class HostedCommandsServiceBuilder : IHostedCommandsServiceBuilder
    {
        private readonly IServiceCollection _services;

        /// <summary>Creates a new instance of the builder.</summary>
        /// <param name="services">Service collection.</param>
        /// <exception cref="ArgumentNullException">Service collection is null.</exception>
        public HostedCommandsServiceBuilder(IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            this._services = services;
        }

        /// <inheritdoc/>
        public IHostedCommandsServiceBuilder Configure(Action<CommandsOptions> configure)
        {
            this._services.Configure<CommandsOptions>(configure);
            return this;
        }

        /// <inheritdoc/>
        public IHostedCommandsServiceBuilder ConfigureArgumentsParser(Action<ArgumentsParserOptions> configure)
        {
            this._services.Configure<ArgumentsParserOptions>(configure);
            return this;
        }

        /// <inheritdoc/>
        public IHostedCommandsServiceBuilder ConfigureCommandInitializerProvider(Action<CommandInitializerProviderOptions> configure)
        {
            this._services.Configure<CommandInitializerProviderOptions>(configure);
            return this;
        }

        /// <inheritdoc/>
        public IHostedCommandsServiceBuilder ConfigureArgumentConverterProvider(Action<ArgumentConverterProviderOptions> configure)
        {
            this._services.Configure<ArgumentConverterProviderOptions>(configure);
            return this;
        }
    }
}
