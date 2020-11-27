using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using TehGM.Wolfringo.Commands;
using TehGM.Wolfringo.Commands.Initialization;
using TehGM.Wolfringo.Commands.Parsing;
using TehGM.Wolfringo.Commands.Attributes;
using TehGM.Wolfringo.Hosting.Commands;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CommandsServiceCollectionExtensions
    {
        /// <summary>Adds Wolfringo Commands System.</summary>
        /// <param name="services">Service collection.</param>
        /// <param name="configure">Alters configuration of Command Service.</param>
        public static IHostedCommandsServiceBuilder AddWolfringoCommands(this IServiceCollection services, Action<CommandsOptions> configure = null)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configure != null)
                services.Configure<CommandsOptions>(configure);

            // add all required services
            services.TryAddSingleton<ICommandHandlerProvider, CommandHandlerProvider>();
            services.TryAddTransient<ICommandsLoader, CommandsLoader>();
            services.TryAdd(ServiceDescriptor.Transient<ICommandInitializerProvider, CommandInitializerProvider>(provider
                => new CommandInitializerProvider(provider.GetRequiredService<IOptionsSnapshot<CommandInitializerProviderOptions>>().Value)));
            services.TryAdd(ServiceDescriptor.Transient<IArgumentsParser, ArgumentsParser>(provider 
                => new ArgumentsParser(provider.GetRequiredService<IOptionsSnapshot<ArgumentsParserOptions>>().Value)));
            services.TryAdd(ServiceDescriptor.Transient<IArgumentConverterProvider, ArgumentConverterProvider>(provider
                => new ArgumentConverterProvider(provider.GetRequiredService<IOptionsSnapshot<ArgumentConverterProviderOptions>>().Value)));

            services.TryAddSingleton<ICommandsService, HostedCommandsService>();
            services.AddTransient<IHostedService>(provider => (IHostedService)provider.GetRequiredService<ICommandsService>());

            return new HostedCommandsServiceBuilder(services);
        }



        // general
        /// <summary>Sets prefix and prefix requirement.</summary>
        /// <param name="builder">Hosted Commands Service builder.</param>
        /// <param name="prefix">Prefix value.</param>
        /// <param name="requirement">Prefix requirement.</param>
        /// <seealso cref="CommandsOptions.Prefix"/>
        /// <seealso cref="CommandsOptions.RequirePrefix"/>
        public static IHostedCommandsServiceBuilder SetPrefix(this IHostedCommandsServiceBuilder builder, string prefix, PrefixRequirement requirement)
            => builder.SetPrefix(prefix).SetPrefixRequirement(requirement);

        /// <summary>Sets case sensitivity.</summary>
        /// <param name="builder">Hosted Commands Service builder.</param>
        /// <param name="caseSensitive">Whether commands should be case sensitive.</param>
        /// <seealso cref="CommandsOptions.CaseSensitivity"/>
        public static IHostedCommandsServiceBuilder SetCaseSensitive(this IHostedCommandsServiceBuilder builder, bool caseSensitive)
            => builder.Configure(options => options.CaseSensitivity = caseSensitive);

        /// <summary>Sets prefix.</summary>
        /// <param name="builder">Hosted Commands Service builder.</param>
        /// <param name="prefix">Prefix value.</param>
        /// <seealso cref="CommandsOptions.Prefix"/>
        public static IHostedCommandsServiceBuilder SetPrefix(this IHostedCommandsServiceBuilder builder, string prefix)
            => builder.Configure(options => options.Prefix = prefix);

        /// <summary>Sets prefix requirement.</summary>
        /// <param name="builder">Hosted Commands Service builder.</param>
        /// <param name="requirement">Prefix requirement.</param>
        /// <seealso cref="CommandsOptions.RequirePrefix"/>
        public static IHostedCommandsServiceBuilder SetPrefixRequirement(this IHostedCommandsServiceBuilder builder, PrefixRequirement requirement)
            => builder.Configure(options => options.RequirePrefix = requirement);



        // for commands loader
        /// <summary>Removes all default assemblies and classes from Commands Options.</summary>
        /// <param name="builder">Hosted Commands Service builder.</param>
        /// <seealso cref="CommandsOptions"/>
        /// <seealso cref="CommandHandlerAttribute"/>
        /// <seealso cref="ICommandsLoader"/>
        public static IHostedCommandsServiceBuilder RemoveDefaultHandlers(this IHostedCommandsServiceBuilder builder)
        {
            // build a default instance of options to get ones that need removing
            CommandsOptions defaultOptions = new CommandsOptions();

            return builder.Configure(options =>
            {
                foreach (Assembly asm in defaultOptions.Assemblies)
                    options.Assemblies.Remove(asm);
                foreach (Type type in defaultOptions.Classes)
                    options.Classes.Remove(type);
            });
        }

        /// <summary>Adds handlers to Command Service.</summary>
        /// <param name="builder">Hosted Commands Service builder.</param>
        /// <param name="handlerTypes">Types of handlers to add.</param>
        /// <seealso cref="CommandHandlerAttribute"/>
        /// <seealso cref="ICommandsLoader.LoadFromTypeAsync(TypeInfo, System.Threading.CancellationToken)"/>
        /// <seealso cref="CommandsOptions.Classes"/>
        public static IHostedCommandsServiceBuilder AddHandlers(this IHostedCommandsServiceBuilder builder, params Type[] handlerTypes)
            => builder.Configure(options =>
            {
                foreach (Type type in handlerTypes)
                    options.Classes.Add(type);
            });

        /// <summary>Adds handler to Command Service.</summary>
        /// <typeparam name="T">Type of handler to add.</typeparam>
        /// <param name="builder">Hosted Commands Service builder.</param>
        /// <seealso cref="CommandHandlerAttribute"/>
        /// <seealso cref="ICommandsLoader.LoadFromTypeAsync(TypeInfo, System.Threading.CancellationToken)"/>
        /// <seealso cref="CommandsOptions.Classes"/>
        public static IHostedCommandsServiceBuilder AddHandler<T>(this IHostedCommandsServiceBuilder builder)
            => AddHandlers(builder, typeof(T));

        /// <summary>Adds handlers to Command Service.</summary>
        /// <param name="assemblies">Assemblies to load handlers from.</param>
        /// <param name="builder">Hosted Commands Service builder.</param>
        /// <seealso cref="CommandHandlerAttribute"/>
        /// <seealso cref="ICommandsLoader.LoadFromAssemblyAsync(Assembly, System.Threading.CancellationToken)"/>
        /// <seealso cref="CommandsOptions.Assemblies"/>
        public static IHostedCommandsServiceBuilder AddHandlers(this IHostedCommandsServiceBuilder builder, params Assembly[] assemblies)
            => builder.Configure(options =>
            {
                foreach (Assembly asm in assemblies)
                    options.Assemblies.Add(asm);
            });



        // for arguments parser
        /// <summary>Adds argument marker to Arguments Parser.</summary>
        /// <param name="builder">Hosted Commands Service builder.</param>
        /// <param name="startMarker">Opening marker for argument block.</param>
        /// <param name="endMarker">Closing marker for argument block.</param>
        /// <seealso cref="IArgumentsParser"/>
        /// <seealso cref="ArgumentsParserOptions.BlockMarkers"/>
        public static IHostedCommandsServiceBuilder AddArgumentBlockMarker(this IHostedCommandsServiceBuilder builder, char startMarker, char endMarker)
            => builder.ConfigureArgumentsParser(options => options.BlockMarkers[startMarker] = endMarker);

        /// <summary>Removes argument marker from Arguments Parser.</summary>
        /// <param name="builder">Hosted Commands Service builder.</param>
        /// <param name="startMarker">Opening marker for argument block.</param>
        /// <seealso cref="IArgumentsParser"/>
        /// <seealso cref="ArgumentsParserOptions.BlockMarkers"/>
        public static IHostedCommandsServiceBuilder RemoveArgumentBlockMarker(this IHostedCommandsServiceBuilder builder, char startMarker)
            => builder.ConfigureArgumentsParser(options => options.BlockMarkers.Remove(startMarker));

        /// <summary>Sets base argument marker for Arguments Parser.</summary>
        /// <param name="builder">Hosted Commands Service builder.</param>
        /// <param name="marker">Marker for to use as base.</param>
        /// <seealso cref="IArgumentsParser"/>
        /// <seealso cref="ArgumentsParserOptions.BaseMarker"/>
        public static IHostedCommandsServiceBuilder SetArgumentBaseMarker(this IHostedCommandsServiceBuilder builder, char marker)
            => builder.ConfigureArgumentsParser(options => options.BaseMarker = marker);



        // for initializers provider
        /// <summary>Maps command initializer for a command attribute type in Command Initializer Provider.</summary>
        /// <param name="builder">Hosted Commands Service builder.</param>
        /// <param name="commandArgumentType">Type of command attribute.</param>
        /// <param name="initializer">Initializer to use for given command attribute type.</param>
        /// <seealso cref="ICommandInitializerProvider"/>
        /// <seealso cref="ICommandInitializer"/>
        /// <seealso cref="CommandAttributeBase"/>
        /// <seealso cref="CommandInitializerProviderOptions.Initializers"/>
        public static IHostedCommandsServiceBuilder MapCommandInitializer(this IHostedCommandsServiceBuilder builder, Type commandArgumentType, ICommandInitializer initializer)
            => builder.ConfigureCommandInitializerProvider(options => options.Initializers[commandArgumentType] = initializer);



        // for argument converter provider
        /// <summary>Sets a fallback enum argument converter in Argument Converter Provider.</summary>
        /// <param name="builder">Hosted Commands Service builder.</param>
        /// <param name="converter">Converter to convert enums with.</param>
        /// <seealso cref="IArgumentConverterProvider"/>
        /// <seealso cref="IArgumentConverter"/>
        /// <seealso cref="ArgumentConverterProviderOptions.EnumConverter"/>
        public static IHostedCommandsServiceBuilder SetEnumArgumentConverter(this IHostedCommandsServiceBuilder builder, IArgumentConverter converter)
            => builder.ConfigureArgumentConverterProvider(options => options.EnumConverter = converter);

        /// <summary>Maps an argument converter in Argument Converter Provider.</summary>
        /// <param name="builder">Hosted Commands Service builder.</param>
        /// <param name="parameterType">Type of parameter to set converter for.</param>
        /// <param name="converter">Converter to convert type with.</param>
        /// <seealso cref="IArgumentConverterProvider"/>
        /// <seealso cref="IArgumentConverter"/>
        /// <seealso cref="ArgumentConverterProviderOptions.Converters"/>
        public static IHostedCommandsServiceBuilder MapArgumentConverter(this IHostedCommandsServiceBuilder builder, Type parameterType, IArgumentConverter converter)
            => builder.ConfigureArgumentConverterProvider(options => options.Converters[parameterType] = converter);
    }
}
