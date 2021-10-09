using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TehGM.Wolfringo.Messages.Responses;
using TehGM.Wolfringo.Messages.Serialization;
using TehGM.Wolfringo.Utilities;
using TehGM.Wolfringo.Utilities.Internal;

namespace TehGM.Wolfringo
{
    /// <summary>A builder for <see cref="WolfClient"/>.</summary>
    public class WolfClientBuilder
    {
        /// <summary>Options for the client.</summary>
        public WolfClientOptions Options { get; set; }
        private readonly IServiceCollection _services;

        /// <summary>Invoked when the builder is about to build a new instance of <see cref="WolfClient"/>.</summary>
        public event Action<IServiceCollection> Building;
        /// <summary>Invoked when the builder has finished building a new instance of <see cref="WolfClient"/>.</summary>
        public event Action<WolfClient> Built;

        /// <summary>Creates a new WolfClient builder with provided services.</summary>
        /// <param name="services">Initial services to use.</param>
        public WolfClientBuilder(IServiceCollection services)
        {
            this.Options = new WolfClientOptions();
            this._services = services;

            // initialize defaults
            if (!this._services.HasService<IWolfTokenProvider>())
                this.WithDefaultTokenProvider();
            if (!this._services.HasService<ISerializerProvider<string, IMessageSerializer>>())
                this.WithDefaultMessageSerializers();
            if (!this._services.HasService<ISerializerProvider<Type, IResponseSerializer>>())
                this.WithDefaultResponseSerializers();
            if (!this._services.HasService<IResponseTypeResolver>())
                this.WithDefaultResponseTypeResolver();
            if (!this._services.HasService<IWolfClientCache>())
                this.WithDefaultCaching();
        }

        /// <summary>Creates a new WolfClient builder with default values pre-set.</summary>
        public WolfClientBuilder()
            : this(new ServiceCollection()) { }

        #region DI HELPERS
        private WolfClientBuilder SetService<TService>(TService service) where TService : class
        {
            this._services.RemoveService<TService>();
            this._services.AddSingleton<TService>(service);
            return this;
        }
        private WolfClientBuilder SetService<TService>(Func<IServiceProvider, TService> factory) where TService : class
        {
            this._services.RemoveService<TService>();
            this._services.AddSingleton<TService>(factory);
            return this;
        }
        private WolfClientBuilder SetService<TService, TImplementation>() where TService : class where TImplementation : class, TService
        {
            this._services.RemoveService<TService>();
            this._services.AddSingleton<TService, TImplementation>();
            return this;
        }
        #endregion

        #region OPTIONS
        // SERVER URL
        /// <summary>Sets WOLF server URL to connect to.</summary>
        /// <param name="url">WOLF server URL to connect to.</param>
        /// <seealso cref="WithDefaultServerURL"/>
        /// <seealso cref="WithBetaServerURL"/>
        /// <returns>Current builder instance.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="url"/> is null, empty or whitespace.</exception>
        public WolfClientBuilder WithServerURL(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException(nameof(url));
            this.Options.ServerURL = url;
            return this;
        }
        /// <summary>Sets WOLF server URL to default value.</summary>
        /// <seealso cref="WolfClientOptions.DefaultServerURL"/>
        /// <seealso cref="WithBetaServerURL"/>
        /// <seealso cref="WithServerURL(string)"/>
        /// <returns>Current builder instance.</returns>
        public WolfClientBuilder WithDefaultServerURL()
            => this.WithServerURL(WolfClientOptions.DefaultServerURL);
        /// <summary>Sets WOLF server URL to beta/rc value.</summary>
        /// <seealso cref="WolfClientOptions.BetaServerURL"/>
        /// <seealso cref="WithDefaultServerURL"/>
        /// <seealso cref="WithServerURL(string)"/>
        /// <returns>Current builder instance.</returns>
        public WolfClientBuilder WithBetaServerURL()
            => this.WithServerURL(WolfClientOptions.BetaServerURL);

        // DEVICE
        /// <summary>Sets device to connect as.</summary>
        /// <param name="device">Device to connect as.</param>
        /// <returns>Current builder instance.</returns>
        public WolfClientBuilder WithDevice(WolfDevice device)
        {
            this.Options.Device = device;
            return this;
        }
        #endregion

        #region TOKEN PROVIDER
        /// <summary>Sets a specific token to use for connection.</summary>
        /// <remarks>Using this method disables <see cref="IWolfTokenProvider"/> set by using <see cref="WithTokenProvider(IWolfTokenProvider)"/> and <see cref="WithDefaultTokenProvider"/>.</remarks>
        /// <param name="token">Token value.</param>
        /// <seealso cref="WithDefaultTokenProvider"/>
        /// <seealso cref="WithTokenProvider(IWolfTokenProvider)"/>
        /// <returns>Current builder instance.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="token"/> is null, empty or whitespace.</exception>
        public WolfClientBuilder WithToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentNullException(nameof(token));
            return this.SetService<IWolfTokenProvider>(_ => new ConstantWolfTokenProvider(token));
        }
        /// <summary>Sets token provider used to generate a token for connection.</summary>
        /// <remarks>Using this method disables token set by using <see cref="WithToken(string)"/>.</remarks>
        /// <param name="provider">Token provider.</param>
        /// <seealso cref="WithDefaultTokenProvider"/>
        /// <seealso cref="WithToken(string)"/>
        /// <returns>Current builder instance.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="provider"/> is null.</exception>
        public WolfClientBuilder WithTokenProvider(IWolfTokenProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            return this.SetService<IWolfTokenProvider>(provider);
        }
        /// <summary>Sets token provider used to generate a token for connection.</summary>
        /// <remarks><para>Using this method disables token set by using <see cref="WithToken(string)"/>.</para>
        /// <para><typeparamref name="TImplementation"/> must have a constructar that can be resolved from provided services.</para></remarks>
        /// <typeparam name="TImplementation">Type of token provider.</typeparam>
        /// <seealso cref="WithDefaultTokenProvider"/>
        /// <seealso cref="WithToken(string)"/>
        /// <returns>Current builder instance.</returns>
        public WolfClientBuilder WithTokenProvider<TImplementation>() where TImplementation : class, IWolfTokenProvider
            => this.SetService<IWolfTokenProvider, TImplementation>();
        /// <summary>Switches to default token provider to generate a token for connection.</summary>
        /// <remarks><para><see cref="RandomizedWolfTokenProvider"/> will be used.</para>
        /// <para>Using this method disables token set by using <see cref="WithToken(string)"/>.</para></remarks>
        /// <seealso cref="WithTokenProvider(IWolfTokenProvider)"/>
        /// <seealso cref="WithToken(string)"/>
        /// <returns>Current builder instance.</returns>
        public WolfClientBuilder WithDefaultTokenProvider()
            => this.WithTokenProvider<RandomizedWolfTokenProvider>();
        #endregion

        #region MESSAGE SERIALIZER
        /// <summary>Sets message serializers provider to use for serializing messages.</summary>
        /// <param name="provider">Serializers provider.</param>
        /// <seealso cref="WithDefaultMessageSerializers()"/>
        /// <returns>Current builder instance.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="provider"/> is null.</exception>
        public WolfClientBuilder WithMessageSerializers(ISerializerProvider<string, IMessageSerializer> provider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            return this.SetService<ISerializerProvider<string, IMessageSerializer>>(provider);
        }
        /// <summary>Sets message serializers provider to use for serializing messages.</summary>
        /// <remarks><typeparamref name="TImplementation"/> must have a constructar that can be resolved from provided services.</remarks>
        /// <typeparam name="TImplementation">Type of serializers provider.</typeparam>
        /// <seealso cref="WithDefaultMessageSerializers()"/>
        /// <returns>Current builder instance.</returns>
        public WolfClientBuilder WithMessageSerializers<TImplementation>() where TImplementation : class, ISerializerProvider<string, IMessageSerializer>
            => this.SetService<ISerializerProvider<string, IMessageSerializer>, TImplementation>();
        /// <summary>Switches to default message serializers provider to use for serializing messages.</summary>
        /// <remarks><see cref="MessageSerializerProvider"/> will be used.</remarks>
        /// <param name="options">Options for the default provider.</param>
        /// <seealso cref="WithMessageSerializers(ISerializerProvider{string, IMessageSerializer})"/>
        /// <returns>Current builder instance.</returns>
        public WolfClientBuilder WithDefaultMessageSerializers(MessageSerializerProviderOptions options)
            => this.SetService<ISerializerProvider<string, IMessageSerializer>>(_ => new MessageSerializerProvider(options));
        /// <summary>Switches to default message serializers provider with default options to use for serializing messages.</summary>
        /// <remarks><see cref="MessageSerializerProvider"/> will be used.</remarks>
        /// <seealso cref="WithMessageSerializers(ISerializerProvider{string, IMessageSerializer})"/>
        /// <returns>Current builder instance.</returns>
        public WolfClientBuilder WithDefaultMessageSerializers()
            => this.WithMessageSerializers<MessageSerializerProvider>();
        #endregion

        #region RESPONSE SERIALIZERS
        /// <summary>Sets response serializers provider to use for deserializing responses.</summary>
        /// <param name="provider">Serializers provider.</param>
        /// <seealso cref="WithDefaultResponseSerializers()"/>
        /// <returns>Current builder instance.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="provider"/> is null.</exception>
        public WolfClientBuilder WithResponseSerializers(ISerializerProvider<Type, IResponseSerializer> provider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            return this.SetService<ISerializerProvider<Type, IResponseSerializer>>(provider);
        }
        /// <summary>Sets response serializers provider to use for deserializing messages.</summary>
        /// <remarks><typeparamref name="TImplementation"/> must have a constructar that can be resolved from provided services.</remarks>
        /// <typeparam name="TImplementation">Type of serializers provider.</typeparam>
        /// <seealso cref="WithDefaultResponseSerializers()"/>
        /// <returns>Current builder instance.</returns>
        public WolfClientBuilder WithResponseSerializers<TImplementation>() where TImplementation : class, ISerializerProvider<Type, IResponseSerializer>
            => this.SetService<ISerializerProvider<Type, IResponseSerializer>, TImplementation>();
        /// <summary>Switches to default response serializers provider to use for deserializing responses.</summary>
        /// <remarks><see cref="ResponseSerializerProvider"/> will be used.</remarks>
        /// <param name="options">Options for the default provider.</param>
        /// <seealso cref="WithResponseSerializers(ISerializerProvider{Type, IResponseSerializer})"/>
        /// <returns>Current builder instance.</returns>
        public WolfClientBuilder WithDefaultResponseSerializers(ResponseSerializerProviderOptions options)
            => this.SetService<ISerializerProvider<Type, IResponseSerializer>>(_ => new ResponseSerializerProvider(options));
        /// <summary>Switches to default response serializers provider with default options to use for deserializing responses.</summary>
        /// <remarks><see cref="ResponseSerializerProvider"/> will be used.</remarks>
        /// <seealso cref="WithResponseSerializers(ISerializerProvider{Type, IResponseSerializer})"/>
        /// <returns>Current builder instance.</returns>
        public WolfClientBuilder WithDefaultResponseSerializers()
            => this.WithResponseSerializers<ResponseSerializerProvider>();
        #endregion

        #region RESPONSE TYPE RESOLVER
        /// <summary>Sets response type resolver to use for deserializing responses.</summary>
        /// <param name="resolver">Response type resolver.</param>
        /// <seealso cref="WithDefaultResponseTypeResolver"/>
        /// <returns>Current builder instance.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="resolver"/> is null.</exception>
        public WolfClientBuilder WithResponseTypeResolver(IResponseTypeResolver resolver)
        {
            if (resolver == null)
                throw new ArgumentNullException(nameof(resolver));
            return this.SetService<IResponseTypeResolver>(resolver);
        }
        /// <summary>Sets response type resolver to use for deserializing responses.</summary>
        /// <remarks><typeparamref name="TImplementation"/> must have a constructar that can be resolved from provided services.</remarks>
        /// <typeparam name="TImplementation">Type of response type resolver.</typeparam>
        /// <seealso cref="WithDefaultResponseTypeResolver"/>
        /// <returns>Current builder instance.</returns>
        public WolfClientBuilder WithResponseTypeResolver<TImplementation>() where TImplementation : class, IResponseTypeResolver
            => this.SetService<IResponseTypeResolver, TImplementation>();
        /// <summary>Switches to default response type resolver to use for deserializing responses.</summary>
        /// <remarks><see cref="ResponseTypeResolver"/> will be used.</remarks>
        /// <seealso cref="WithResponseTypeResolver(IResponseTypeResolver)"/>
        /// <returns>Current builder instance.</returns>
        public WolfClientBuilder WithDefaultResponseTypeResolver()
            => this.SetService<IResponseTypeResolver, ResponseTypeResolver>();
        #endregion

        #region CACHING
        /// <summary>Sets entity cache container that client will use.</summary>
        /// <param name="cache">The entity cache container.</param>
        /// <returns>Current builder instance.</returns>
        public WolfClientBuilder WithCaching(IWolfClientCache cache)
        {
            if (cache == null)
                throw new ArgumentNullException(nameof(cache));
            return this.SetService<IWolfClientCache>(cache);
        }

        /// <summary>Sets entity cache container that client will use.</summary>
        /// <typeparam name="TImplementation">Type of entity cache container.</typeparam>
        /// <returns>Current builder instance.</returns>
        public WolfClientBuilder WithCaching<TImplementation>() where TImplementation : class, IWolfClientCache
            => this.SetService<IWolfClientCache, TImplementation>();

        /// <summary>Switches to default entity cache container.</summary>
        /// <remarks><see cref="WolfEntityCacheContainer"/> will be used.</remarks>
        /// <param name="options">Options for the default entity cache container.</param>
        /// <returns>Current builder instance.</returns>
        public WolfClientBuilder WithDefaultCaching(WolfCacheOptions options)
            => this.SetService<IWolfClientCache>(provider => new WolfEntityCacheContainer(options, provider.GetLoggerFor<IWolfClientCache, WolfEntityCacheContainer>()));

        /// <summary>Switches to default entity cache container with all caches enabled.</summary>
        /// <remarks><see cref="WolfEntityCacheContainer"/> will be used.</remarks>
        /// <returns>Current builder instance.</returns>
        public WolfClientBuilder WithDefaultCaching()
            => this.WithDefaultCaching(new WolfCacheOptions());
        #endregion

        #region LOGGING
        /// <summary>Sets a logger to be used by the client.</summary>
        /// <param name="logger">Logger to use.</param>
        /// <returns>Current builder instance.</returns>
        public WolfClientBuilder WithLogging(ILogger logger)
            => this.SetService<ILogger>(logger);
        /// <summary>Sets a logger factory that will be used to create a logger.</summary>
        /// <param name="factory">Logger factory to use when creating a logger.</param>
        /// <returns>Current builder instance.</returns>
        public WolfClientBuilder WithLogging(ILoggerFactory factory)
            => this.SetService<ILogger>(_ => factory.CreateLogger<WolfClient>());
        #endregion

        /// <summary>Builds a new WolfClient with provided values.</summary>
        /// <returns>A new WolfClient instance</returns>
        /// <exception cref="ArgumentNullException">Server URL is null.</exception>
        public WolfClient Build()
        {
            if (string.IsNullOrWhiteSpace(this.Options.ServerURL))
                throw new ArgumentNullException(nameof(this.Options.ServerURL));

            // add options before building
            this.SetService<WolfClientOptions>(this.Options);

            // build and return
            this.Building?.Invoke(this._services);
            WolfClient result = new WolfClient(this._services.BuildServiceProvider(), this.Options);
            this.Built?.Invoke(result);
            return result;
        }
    }
}
