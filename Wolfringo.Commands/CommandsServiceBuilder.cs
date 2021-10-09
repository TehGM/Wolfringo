using System;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TehGM.Wolfringo.Commands.Initialization;
using TehGM.Wolfringo.Commands.Parsing;
using TehGM.Wolfringo.Utilities.Internal;

namespace TehGM.Wolfringo.Commands
{
    /// <summary>A builder for <see cref="CommandsService"/>.</summary>
    public class CommandsServiceBuilder
    {
        /// <summary>Options for the client.</summary>
        public CommandsOptions Options { get; set; }
        private readonly IServiceCollection _services;
        private readonly DisposableServicesHandler _disposablesHandler;
        private CancellationToken _cancellationToken;

        /// <summary>Invoked when the builder is about to build a new instance of <see cref="CommandsService"/>.</summary>
        public event Action<IServiceCollection> Building;
        /// <summary>Invoked when the builder has finished building a new instance of <see cref="CommandsService"/>.</summary>
        public event Action<CommandsService, IServiceProvider> Built;

        /// <summary>Creates a new CommandsService builder with provided services.</summary>
        /// <param name="services">Initial services to use.</param>
        public CommandsServiceBuilder(IServiceCollection services)
        {
            this.Options = new CommandsOptions();
            this._services = services;
            this._disposablesHandler = new DisposableServicesHandler();
            this._cancellationToken = default;

            // initialize defaults
            if (!this._services.HasService<IArgumentsParser>())
                this.WithDefaultArgumentsParser();
            if (!this._services.HasService<IArgumentConverterProvider>())
                this.WithDefaultArgumentsConverterProvider();
            if (!this._services.HasService<ICommandInitializerProvider>())
                this.WithDefaultCommandInitializerProvider();
            if (!this._services.HasService<ICommandsHandlerProvider>())
                this.WithDefaultCommandsHandlerProvider();
            if (!this._services.HasService<ICommandsLoader>())
                this.WithDefaultCommandsLoader();
            if (!this._services.HasService<IParameterBuilder>())
                this.WithDefaultParameterBuilder();
        }

        /// <summary>Creates a new WolfClient builder with default values pre-set.</summary>
        public CommandsServiceBuilder()
            : this(new ServiceCollection()) { }

        #region CUSTOM SERVICES
        // SINGLETON
        /// <summary>Adds any custom service so it can be resolved during Dependency Injection.</summary>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <param name="service">The service instance.</param>
        /// <returns>Current builder instance.</returns>
        public CommandsServiceBuilder WithSingletonService<TService>(TService service) where TService : class
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service));

            this._disposablesHandler.UnmarkForDisposal<TService>();
            this._services.RemoveService<TService>();
            this._services.AddSingleton<TService>(service);
            return this;
        }
        /// <summary>Adds any custom service so it can be resolved during Dependency Injection.</summary>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <param name="factory">Delegate that will be invoked when the service is resolved.</param>
        /// <returns>Current builder instance.</returns>
        public CommandsServiceBuilder WithSingletonService<TService>(Func<IServiceProvider, TService> factory) where TService : class
        {
            this._disposablesHandler.MarkForDisposal<TService>();
            this._services.RemoveService<TService>();
            this._services.AddSingleton<TService>(factory);
            return this;
        }
        /// <summary>Adds any custom service so it can be resolved during Dependency Injection.</summary>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <typeparam name="TImplementation">Implementation type of the service</typeparam>
        /// <returns>Current builder instance.</returns>
        public CommandsServiceBuilder WithSingletonService<TService, TImplementation>() where TService : class where TImplementation : class, TService
        {
            this._disposablesHandler.MarkForDisposal<TService>();
            this._services.RemoveService<TService>();
            this._services.AddSingleton<TService, TImplementation>();
            return this;
        }

        // SCOPED
        /// <summary>Adds any custom service so it can be resolved during Dependency Injection.</summary>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <param name="factory">Delegate that will be invoked when the service is resolved.</param>
        /// <returns>Current builder instance.</returns>
        public CommandsServiceBuilder WithScopedService<TService>(Func<IServiceProvider, TService> factory) where TService : class
        {
            this._disposablesHandler.UnmarkForDisposal<TService>();
            this._services.RemoveService<TService>();
            this._services.AddScoped<TService>(factory);
            return this;
        }
        /// <summary>Adds any custom service so it can be resolved during Dependency Injection.</summary>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <typeparam name="TImplementation">Implementation type of the service</typeparam>
        /// <returns>Current builder instance.</returns>
        public CommandsServiceBuilder WithScopedService<TService, TImplementation>() where TService : class where TImplementation : class, TService
        {
            this._disposablesHandler.UnmarkForDisposal<TService>();
            this._services.RemoveService<TService>();
            this._services.AddScoped<TService, TImplementation>();
            return this;
        }

        // TRANSIENT
        /// <summary>Adds any custom service so it can be resolved during Dependency Injection.</summary>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <param name="factory">Delegate that will be invoked when the service is resolved.</param>
        /// <returns>Current builder instance.</returns>
        public CommandsServiceBuilder WithTransientService<TService>(Func<IServiceProvider, TService> factory) where TService : class
        {
            this._disposablesHandler.UnmarkForDisposal<TService>();
            this._services.RemoveService<TService>();
            this._services.AddTransient<TService>(factory);
            return this;
        }
        /// <summary>Adds any custom service so it can be resolved during Dependency Injection.</summary>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <typeparam name="TImplementation">Implementation type of the service</typeparam>
        /// <returns>Current builder instance.</returns>
        public CommandsServiceBuilder WithTransientService<TService, TImplementation>() where TService : class where TImplementation : class, TService
        {
            this._disposablesHandler.UnmarkForDisposal<TService>();
            this._services.RemoveService<TService>();
            this._services.AddTransient<TService, TImplementation>();
            return this;
        }
        #endregion

        #region WOLF CLIENT
        /// <summary>Sets WOLF client to use with the <see cref="CommandsService"/>.</summary>
        /// <param name="client">The WOLF client.</param>
        /// <returns>Current builder instance.</returns>
        public CommandsServiceBuilder WithWolfClient(IWolfClient client)
            => this.WithSingletonService<IWolfClient>(client);

        /// <summary>Sets WOLF client to use with the <see cref="CommandsService"/>.</summary>
        /// <typeparam name="TImplementation">Implementation type of the WOLF client.</typeparam>
        /// <returns>Current builder instance.</returns>
        public CommandsServiceBuilder WithWolfClient<TImplementation>() where TImplementation : class, IWolfClient
            => this.WithSingletonService<IWolfClient, TImplementation>();

        /// <summary>Sets WOLF client to use with the <see cref="CommandsService"/>.</summary>
        /// <remarks>This method allows you configure the client using <see cref="WolfClientBuilder"/>.</remarks>
        /// <param name="clientBuilder">A delegate that allows configuring the <see cref="WolfClient"/> before creation.</param>
        /// <returns>Current builder instance.</returns>
        public CommandsServiceBuilder WithWolfClient(Action<WolfClientBuilder> clientBuilder)
        {
            WolfClientBuilder builder = new WolfClientBuilder(this._services);
            clientBuilder?.Invoke(builder);
            return this.WithSingletonService<IWolfClient>(_ => builder.Build());
        }
        #endregion

        #region OPTIONS
        /// <summary>Sets the default prefix for commands.</summary>
        /// <param name="prefix">Prefix value.</param>
        /// <returns>Current builder instance.</returns>
        public CommandsServiceBuilder WithPrefix(string prefix)
            => this.ConfigureOptions(options => options.Prefix = prefix);
        /// <summary>Sets whether commands are case-sensitive by default.</summary>
        /// <param name="caseSensitive">Case sensitivity setting.</param>
        /// <returns>Current builder instance.</returns>
        public CommandsServiceBuilder WithCaseSensitivity(bool caseSensitive)
            => this.ConfigureOptions(options => options.CaseSensitivity = caseSensitive);
        /// <summary>Sets default prefix requirement for commands.</summary>
        /// <param name="requirePrefix">Prefix requirement.</param>
        /// <returns>Current builder instance.</returns>
        public CommandsServiceBuilder WithPrefixRequirement(PrefixRequirement requirePrefix)
            => this.ConfigureOptions(options => options.RequirePrefix = requirePrefix);
        /// <summary>Enables or disables default help command functionality.</summary>
        /// <param name="enabled">Whether the default help command should be enabled.</param>
        /// <returns>Current builder instance.</returns>
        public CommandsServiceBuilder WithDefaultHelpCommand(bool enabled)
            => this.ConfigureOptions(options => options.EnableDefaultHelpCommand = enabled);
        /// <summary>Enables default help command functionality.</summary>
        /// <returns>Current builder instance.</returns>
        public CommandsServiceBuilder WithDefaultHelpCommand()
            => this.WithDefaultHelpCommand(true);

        /// <summary>Allows configuring options for commands service.</summary>
        /// <param name="configure">Delegate that can be used for options configuration.</param>
        /// <returns>Current builder instance.</returns>
        public CommandsServiceBuilder ConfigureOptions(Action<CommandsOptions> configure)
        {
            configure.Invoke(this.Options);
            return this;
        }
        #endregion

        #region ARGUMENTS PARSER
        /// <summary>Sets arguments parser.</summary>
        /// <param name="parser">The arguments parser.</param>
        /// <returns>Current builder instance.</returns>
        public CommandsServiceBuilder WithArgumentsParser(IArgumentsParser parser)
            => this.WithSingletonService<IArgumentsParser>(parser);

        /// <summary>Sets arguments parser.</summary>
        /// <typeparam name="TImplementation">Implementation type of the arguments parser.</typeparam>
        /// <returns>Current builder instance.</returns>
        public CommandsServiceBuilder WithArgumentsParser<TImplementation>() where TImplementation : class, IArgumentsParser
            => this.WithSingletonService<IArgumentsParser, TImplementation>();

        /// <summary>Switches to default arguments parser.</summary>
        /// <param name="options">Options for the default parser.</param>
        /// <returns>Current builder instance.</returns>
        public CommandsServiceBuilder WithDefaultArgumentsParser(ArgumentsParserOptions options)
            => this.WithSingletonService<IArgumentsParser>(_ => new ArgumentsParser(options));

        /// <summary>Switches to default arguments parser with default options.</summary>
        /// <returns>Current builder instance.</returns>
        public CommandsServiceBuilder WithDefaultArgumentsParser()
            => this.WithArgumentsParser<ArgumentsParser>();
        #endregion

        #region ARGUMENTS CONVERTER PROVIDER
        /// <summary>Sets argument converter provider.</summary>
        /// <param name="provider">The argument converter provider.</param>
        /// <returns>Current builder instance.</returns>
        public CommandsServiceBuilder WithArgumentsConverterProvider(IArgumentConverterProvider provider)
            => this.WithSingletonService<IArgumentConverterProvider>(provider);

        /// <summary>Sets argument converter provider.</summary>
        /// <typeparam name="TImplementation">Implementation type of argument converter provider.</typeparam>
        /// <returns>Current builder instance.</returns>
        public CommandsServiceBuilder WithArgumentsConverterProvider<TImplementation>() where TImplementation : class, IArgumentConverterProvider
            => this.WithSingletonService<IArgumentConverterProvider, TImplementation>();

        /// <summary>Switches to default argument converter provider.</summary>
        /// <param name="options">Options for the argument converter provider.</param>
        /// <returns>Current builder instance.</returns>
        public CommandsServiceBuilder WithDefaultArgumentsConverterProvider(ArgumentConverterProviderOptions options)
            => this.WithSingletonService<IArgumentConverterProvider>(_ => new ArgumentConverterProvider(options));

        /// <summary>Switches to default argument converter provider with default options.</summary>
        /// <returns>Current builder instance.</returns>
        public CommandsServiceBuilder WithDefaultArgumentsConverterProvider()
            => this.WithArgumentsConverterProvider<ArgumentConverterProvider>();
        #endregion

        #region COMMANDS HANDLER PROVIDER
        /// <summary>Sets commands handler provider.</summary>
        /// <param name="provider">The commands handler provider.</param>
        /// <returns>Current builder instance.</returns>
        public CommandsServiceBuilder WithCommandsHandlerProvider(ICommandsHandlerProvider provider)
            => this.WithSingletonService<ICommandsHandlerProvider>(provider);

        /// <summary>Sets commands handler provider.</summary>
        /// <typeparam name="TImplementation">Implementation type of commands handler provider.</typeparam>
        /// <returns>Current builder instance.</returns>
        public CommandsServiceBuilder WithCommandsHandlerProvider<TImplementation>() where TImplementation : class, ICommandsHandlerProvider
            => this.WithSingletonService<ICommandsHandlerProvider, TImplementation>();

        /// <summary>Switches to default commands handler provider.</summary>
        /// <returns>Current builder instance.</returns>
        public CommandsServiceBuilder WithDefaultCommandsHandlerProvider()
            => this.WithCommandsHandlerProvider<CommandsHandlerProvider>();
        #endregion

        #region PARAMETER BUILDER
        /// <summary>Sets parameter builder.</summary>
        /// <param name="builder">The parameter builder.</param>
        /// <returns>Current builder instance.</returns>
        public CommandsServiceBuilder WithParameterBuilder(IParameterBuilder builder)
            => this.WithSingletonService<IParameterBuilder>(builder);

        /// <summary>Sets parameter builder.</summary>
        /// <typeparam name="TImplementation">Implementation type of the parameter builder.</typeparam>
        /// <returns>Current builder instance.</returns>
        public CommandsServiceBuilder WithParameterBuilder<TImplementation>() where TImplementation : class, IParameterBuilder
            => this.WithSingletonService<IParameterBuilder, TImplementation>();

        /// <summary>Switches to default parameter builder.</summary>
        /// <returns>Current builder instance.</returns>
        public CommandsServiceBuilder WithDefaultParameterBuilder()
            => this.WithParameterBuilder<ParameterBuilder>();
        #endregion

        #region COMMAND INITIALIZER PROVIDER
        /// <summary>Sets command initializer provider.</summary>
        /// <param name="provider">The command initializer provider.</param>
        /// <returns>Current builder instance.</returns>
        public CommandsServiceBuilder WithCommandInitializerProvider(ICommandInitializerProvider provider)
            => this.WithSingletonService<ICommandInitializerProvider>(provider);

        /// <summary>Sets command initializer provider.</summary>
        /// <typeparam name="TImplementation">Implementation type of the command initializer provider.</typeparam>
        /// <returns>Current builder instance.</returns>
        public CommandsServiceBuilder WithCommandInitializerProvider<TImplementation>() where TImplementation : class, ICommandInitializerProvider
            => this.WithSingletonService<ICommandInitializerProvider, TImplementation>();

        /// <summary>Switches to default command initializer provider.</summary>
        /// <param name="options">Options for the command initializer provider.</param>
        /// <returns>Current builder instance.</returns>
        public CommandsServiceBuilder WithDefaultCommandInitializerProvider(CommandInitializerProviderOptions options)
            => this.WithSingletonService<ICommandInitializerProvider>(_ => new CommandInitializerProvider(options));

        /// <summary>Switches to default command initializer provider with default options.</summary>
        /// <returns>Current builder instance.</returns>
        public CommandsServiceBuilder WithDefaultCommandInitializerProvider()
            => this.WithCommandInitializerProvider<CommandInitializerProvider>();
        #endregion

        #region COMMANDS LOADER
        /// <summary>Sets commands loader.</summary>
        /// <param name="loader">The commands loader.</param>
        /// <returns>Current builder instance.</returns>
        public CommandsServiceBuilder WithCommandsLoader(ICommandsLoader loader)
            => this.WithSingletonService<ICommandsLoader>(loader);

        /// <summary>Sets commands loader.</summary>
        /// <typeparam name="TImplementation">Implementation type of the commands loader.</typeparam>
        /// <returns>Current builder instance.</returns>
        public CommandsServiceBuilder WithCommandsLoader<TImplementation>() where TImplementation : class, ICommandsLoader
            => this.WithSingletonService<ICommandsLoader, TImplementation>();

        /// <summary>Switches to default commands loader.</summary>
        /// <returns>Current builder instance.</returns>
        public CommandsServiceBuilder WithDefaultCommandsLoader()
            => this.WithSingletonService<ICommandsLoader>(provider => new CommandsLoader(provider.GetRequiredService<ICommandInitializerProvider>(),
                provider.GetLoggerFor<ICommandsLoader, CommandsLoader>()));
        #endregion

        #region LOGGING
        /// <summary>Sets a logger to be used by the <see cref="CommandsService"/>.</summary>
        /// <param name="logger">Logger to use.</param>
        /// <returns>Current builder instance.</returns>
        public CommandsServiceBuilder WithLogging(ILogger logger)
        {
            this._services.RemoveService<ILoggerFactory>();
            if (logger is ILogger<CommandsService> typedLogger)
                this.WithSingletonService<ILogger<CommandsService>>(typedLogger);
            if (logger is ILogger<ICommandsService> interfaceTypedLogger)
                this.WithSingletonService<ILogger<ICommandsService>>(interfaceTypedLogger);
            return this.WithSingletonService<ILogger>(logger);
        }
        /// <summary>Sets a logger factory that will be used to create a logger.</summary>
        /// <param name="factory">Logger factory to use when creating a logger.</param>
        /// <returns>Current builder instance.</returns>
        public CommandsServiceBuilder WithLogging(ILoggerFactory factory)
        {
            this.WithSingletonService<ILogger<CommandsService>>(_ => factory.CreateLogger<CommandsService>());
            this.WithSingletonService<ILogger<ICommandsService>>(_ => factory.CreateLogger<ICommandsService>());
            this.WithSingletonService<ILogger>(_ => factory.CreateLogger<CommandsService>());
            return this.WithSingletonService<ILoggerFactory>(factory);
        }
        #endregion

        #region CANCELLATION TOKEN
        /// <summary>Sets cancellation token that can be used for cancelling all tasks within <see cref="CommandsService"/>.</summary>
        /// <param name="cancellationToken">Cancellation token to use.</param>
        /// <returns>Current builder instance.</returns>
        public CommandsServiceBuilder WithCancellationToken(CancellationToken cancellationToken)
        {
            this._cancellationToken = cancellationToken;
            return this;
        }
        #endregion

        /// <summary>Builds a new CommandsService with provided values.</summary>
        /// <param name="client">The WOLF Client that is used by the built CommandsService.</param>
        /// <param name="services">The built service provider.</param>
        /// <returns>A new CommandsService instance.</returns>
        public CommandsService Build(out IWolfClient client, out IServiceProvider services)
        {
            if (!this._services.HasService<IWolfClient>())
                throw new InvalidOperationException($"Cannot create commands service without WOLF Client. Please use {nameof(this.WithWolfClient)} before calling {nameof(this.Build)}.");

            // add options and disposables handler before building
            this.WithSingletonService<CommandsOptions>(this.Options);
            this.WithSingletonService<DisposableServicesHandler>(this._disposablesHandler);

            // register self result
            this.Building?.Invoke(this._services);
            this._services.AddSingleton<CommandsService>(provider => new CommandsService(provider, this.Options));
            this._services.AddSingleton<ICommandsService>(provider => provider.GetRequiredService<CommandsService>());

            // build and return
            services = this._services.BuildServiceProvider();
            client = services.GetRequiredService<IWolfClient>();
            CommandsService result = services.GetRequiredService<CommandsService>();
            this.Built?.Invoke(result, services);
            return result;
        }

        /// <summary>Builds a new CommandsService with provided values.</summary>
        /// <returns>A new CommandsService instance.</returns>
        public CommandsService Build()
            => this.Build(out _, out _);
    }
}
