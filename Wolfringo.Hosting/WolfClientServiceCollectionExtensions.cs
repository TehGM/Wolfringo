using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;
using TehGM.Wolfringo;
using TehGM.Wolfringo.Hosting;
using TehGM.Wolfringo.Messages.Responses;
using TehGM.Wolfringo.Messages.Serialization;
using TehGM.Wolfringo.Utilities;
using TehGM.Wolfringo.Messages;
using Microsoft.Extensions.Options;
using TehGM.Wolfringo.Utilities.Internal;
using TehGM.Wolfringo.Socket;
using TehGM.Wolfringo.Hosting.Services;
using TehGM.Wolfringo.Caching;
using TehGM.Wolfringo.Caching.Internal;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>Dependency Injection extensions for <see cref="HostedWolfClient"/>.</summary>
    public static class WolfClientServiceCollectionExtensions
    {
        /// <summary>Adds Hosted Wolf Client to services as a hosted service, and all related services.</summary>
        /// <remarks><para>This method will also add <see cref="IWolfTokenProvider"/>, <see cref="IResponseTypeResolver"/>,
        /// and <see cref="ISerializerProvider{TKey, TSerializer}"/> for messages and responses, unless already added.</para>
        /// <para>Added client will be injectable as both <see cref="HostedWolfClient"/> and <see cref="IWolfClient"/>.</para></remarks>
        /// <param name="services">Service collection to add new services to.</param>
        /// <param name="configureOptions">Configuration of client options.</param>
        public static IHostedWolfClientServiceBuilder AddWolfClient(this IServiceCollection services, Action<HostedWolfClientOptions> configureOptions = null)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.TryAddTransient<IWolfTokenProvider>(provider
                => new HostedWolfTokenProvider(provider.GetRequiredService<IOptionsMonitor<HostedWolfClientOptions>>(), new RandomizedWolfTokenProvider()));
            services.TryAddTransient<IResponseTypeResolver, ResponseTypeResolver>();
            services.TryAdd(ServiceDescriptor.Singleton<ISerializerProvider<string, IMessageSerializer>, MessageSerializerProvider>(provider
                => new MessageSerializerProvider(provider.GetRequiredService<IOptions<MessageSerializerProviderOptions>>().Value)));
            services.TryAdd(ServiceDescriptor.Singleton<ISerializerProvider<Type, IResponseSerializer>, ResponseSerializerProvider>(provider
                => new ResponseSerializerProvider(provider.GetRequiredService<IOptions<ResponseSerializerProviderOptions>>().Value)));
            services.TryAdd(ServiceDescriptor.Singleton<IWolfClientCache, WolfEntityCacheContainer>(provider
                => new WolfEntityCacheContainer(provider.GetRequiredService<WolfCacheOptions>(), provider.GetLoggerFor<IWolfClientCache, WolfEntityCacheContainer>())));
            services.TryAddTransient<WolfCacheOptions>(provider
                => provider.GetRequiredService<IOptionsMonitor<WolfCacheOptions>>().CurrentValue);

            services.TryAddSingleton<ISocketClient, SocketClient>();
            services.TryAddSingleton<IWolfClient, HostedWolfClient>();
            services.AddTransient<IHostedService>(x => (IHostedService)x.GetRequiredService<IWolfClient>());

            services.AddOptions();
            if (configureOptions != null)
                services.Configure(configureOptions);

            return new HostedWolfClientServiceBuilder(services);
        }



        // general
        /// <summary>Sets login credentials.</summary>
        /// <param name="builder">Hosted WOLF Client Service builder.</param>
        /// <param name="login">Login.</param>
        /// <param name="password">Password.</param>
        /// <param name="loginType">Login Type.</param>
        /// <seealso cref="HostedWolfClientOptions.LoginUsername"/>
        /// <seealso cref="HostedWolfClientOptions.LoginPassword"/>
        /// <seealso cref="HostedWolfClientOptions.LoginType"/>
        public static IHostedWolfClientServiceBuilder SetCredentials(this IHostedWolfClientServiceBuilder builder, string login, string password, WolfLoginType loginType = WolfLoginType.Email)
            => builder.Configure(options =>
            {
                options.LoginUsername = login;
                options.LoginPassword = password;
                options.LoginType = loginType;
            });

        /// <summary>Sets auto-reconnect behaviour.</summary>
        /// <param name="builder">Hosted WOLF Client Service builder.</param>
        /// <param name="attempts">Max reconnect attempts. 0 to disable. Negative values to infinite.</param>
        /// <param name="delay">Delay between autoreconnect attempts.</param>
        /// <seealso cref="HostedWolfClientOptions.AutoReconnectAttempts"/>
        /// <seealso cref="HostedWolfClientOptions.AutoReconnectDelay"/>
        public static IHostedWolfClientServiceBuilder SetAutoReconnect(this IHostedWolfClientServiceBuilder builder, int attempts, TimeSpan delay)
            => builder.SetAutoReconnectDelay(delay).SetAutoReconnectAttempts(attempts);

        /// <summary>Sets delay between auto-reconnect attempts.</summary>
        /// <param name="builder">Hosted WOLF Client Service builder.</param>
        /// <param name="delay">Delay between autoreconnect attempts.</param>
        /// <seealso cref="HostedWolfClientOptions.AutoReconnectDelay"/>
        public static IHostedWolfClientServiceBuilder SetAutoReconnectDelay(this IHostedWolfClientServiceBuilder builder, TimeSpan delay)
            => builder.Configure(options => options.AutoReconnectDelay = delay);

        /// <summary>Sets max auto-reconnect attempts.</summary>
        /// <param name="builder">Hosted WOLF Client Service builder.</param>
        /// <param name="attempts">Max reconnect attempts. 0 to disable. Negative values to infinite.</param>
        /// <seealso cref="HostedWolfClientOptions.AutoReconnectAttempts"/>
        public static IHostedWolfClientServiceBuilder SetAutoReconnectAttempts(this IHostedWolfClientServiceBuilder builder, int attempts)
            => builder.Configure(options => options.AutoReconnectAttempts = attempts);

        /// <summary>Sets infinite auto-reconnect attempts.</summary>
        /// <param name="builder">Hosted WOLF Client Service builder.</param>
        /// <seealso cref="HostedWolfClientOptions.AutoReconnectAttempts"/>
        public static IHostedWolfClientServiceBuilder SetInfiniteAutoReconnectAttempts(this IHostedWolfClientServiceBuilder builder)
            => SetAutoReconnectAttempts(builder, -1);

        /// <summary>Disables auto-reconnect behaviour.</summary>
        /// <param name="builder">Hosted WOLF Client Service builder.</param>
        /// <seealso cref="HostedWolfClientOptions.AutoReconnectAttempts"/>
        public static IHostedWolfClientServiceBuilder DisableAutoReconnect(this IHostedWolfClientServiceBuilder builder)
            => SetAutoReconnectAttempts(builder, 0);

        /// <summary>Sets server URL to connect to.</summary>
        /// <param name="builder">Hosted WOLF Client Service builder.</param>
        /// <param name="url">URL of WOLF servers.</param>
        /// <seealso cref="HostedWolfClientOptions.ServerURL"/>
        public static IHostedWolfClientServiceBuilder SetServerURL(this IHostedWolfClientServiceBuilder builder, string url)
            => builder.Configure(options => options.ServerURL = url);

        /// <summary>Sets server URL to Default.</summary>
        /// <param name="builder">Hosted WOLF Client Service builder.</param>
        /// <seealso cref="HostedWolfClientOptions.ServerURL"/>
        /// <seealso cref="WolfClientOptions.DefaultServerURL"/>
        public static IHostedWolfClientServiceBuilder SetDefaultServerURL(this IHostedWolfClientServiceBuilder builder)
            => SetServerURL(builder, WolfClientOptions.DefaultServerURL);

        /// <summary>Sets server URL to Release Candidate server.</summary>
        /// <param name="builder">Hosted WOLF Client Service builder.</param>
        /// <seealso cref="HostedWolfClientOptions.ServerURL"/>
        /// <seealso cref="WolfClientOptions.BetaServerURL"/>
        public static IHostedWolfClientServiceBuilder SetBetaServerURL(this IHostedWolfClientServiceBuilder builder)
            => SetServerURL(builder, WolfClientOptions.BetaServerURL);


        // message serializers
        /// <summary>Sets fallback message serializer in Message Serializer Provider.</summary>
        /// <param name="builder">Hosted WOLF Client Service builder.</param>
        /// <param name="serializer">Message serializer to fall back to.</param>
        /// <seealso cref="MessageSerializerProvider"/>
        /// <seealso cref="ISerializerProvider{TKey, TSerializer}"/>
        /// <seealso cref="IMessageSerializer"/>
        /// <seealso cref="MessageSerializerProviderOptions.FallbackSerializer"/>
        public static IHostedWolfClientServiceBuilder SetFallbackMessageSerializer(this IHostedWolfClientServiceBuilder builder, IMessageSerializer serializer)
        {
            if (serializer == null)
                throw new ArgumentNullException(nameof(serializer));
            return builder.ConfigureMessageSerializerProvider(options => options.FallbackSerializer = serializer);
        }

        /// <summary>Maps a message serializer in Message Serializer Provider.</summary>
        /// <param name="builder">Hosted WOLF Client Service builder.</param>
        /// <param name="eventName">Name of WOLF protocol event. <see cref="MessageEventNames"/> for known constants.</param>
        /// <param name="serializer">Serializer to serialize and deserialize with.</param>
        /// <seealso cref="MessageSerializerProvider"/>
        /// <seealso cref="ISerializerProvider{TKey, TSerializer}"/>
        /// <seealso cref="IMessageSerializer"/>
        /// <seealso cref="MessageSerializerProviderOptions.Serializers"/>
        public static IHostedWolfClientServiceBuilder MapMessageSerializer(this IHostedWolfClientServiceBuilder builder, string eventName, IMessageSerializer serializer)
            => builder.ConfigureMessageSerializerProvider(options => options.Serializers[eventName] = serializer);



        // response serializers
        /// <summary>Sets fallback response serializer in Message Response Provider.</summary>
        /// <param name="builder">Hosted WOLF Client Service builder.</param>
        /// <param name="serializer">Response serializer to fall back to.</param>
        /// <seealso cref="ResponseSerializerProvider"/>
        /// <seealso cref="ISerializerProvider{TKey, TSerializer}"/>
        /// <seealso cref="IResponseSerializer"/>
        /// <seealso cref="ResponseSerializerProviderOptions.FallbackSerializer"/>
        public static IHostedWolfClientServiceBuilder SetFallbackResponseSerializer(this IHostedWolfClientServiceBuilder builder, IResponseSerializer serializer)
        {
            if (serializer == null)
                throw new ArgumentNullException(nameof(serializer));
            return builder.ConfigureResponseSerializerProvider(options => options.FallbackSerializer = serializer);
        }

        /// <summary>Maps a response serializer in Message Response Provider.</summary>
        /// <param name="builder">Hosted WOLF Client Service builder.</param>
        /// <param name="responseType">Type of response message.</param>
        /// <param name="serializer">Serializer to serialize and deserialize with.</param>
        /// <seealso cref="ResponseSerializerProvider"/>
        /// <seealso cref="ISerializerProvider{TKey, TSerializer}"/>
        /// <seealso cref="IResponseSerializer"/>
        /// <seealso cref="ResponseSerializerProviderOptions.Serializers"/>
        public static IHostedWolfClientServiceBuilder MapResponseSerializer(this IHostedWolfClientServiceBuilder builder, Type responseType, IResponseSerializer serializer)
            => builder.ConfigureResponseSerializerProvider(options => options.Serializers[responseType] = serializer);
    }
}
