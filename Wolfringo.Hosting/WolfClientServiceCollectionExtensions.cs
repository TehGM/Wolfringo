using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;
using TehGM.Wolfringo;
using TehGM.Wolfringo.Hosting;
using TehGM.Wolfringo.Messages.Responses;
using TehGM.Wolfringo.Messages.Serialization;
using TehGM.Wolfringo.Utilities;
using TehGM.Wolfringo.Messages;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class WolfClientServiceCollectionExtensions
    {
        /// <summary>Adds Hosted Wolf Client to services as a hosted service, and all related services.</summary>
        /// <remarks><para>This method will also add <see cref="ITokenProvider"/>, <see cref="IResponseTypeResolver"/>,
        /// and <see cref="ISerializerProvider{TKey, TSerializer}"/> for messages and responses, unless already added.</para>
        /// <para>Added client will be injectable as both <see cref="IHostedWolfClient"/> and <see cref="IWolfClient"/>.</para></remarks>
        /// <param name="configureOptions">Configuration of client options.</param>
        public static IHostedWolfClientServiceBuilder AddWolfClient(this IServiceCollection services, Action<HostedWolfClientOptions> configureOptions = null)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.TryAddTransient<ITokenProvider, DefaultWolfTokenProvider>();
            services.TryAddTransient<IResponseTypeResolver, DefaultResponseTypeResolver>();
            services.TryAddTransient<ISerializerProvider<string, IMessageSerializer>, MessageSerializerProvider>();
            services.TryAddTransient<ISerializerProvider<Type, IResponseSerializer>, ResponseSerializerProvider>();

            services.TryAddSingleton<IWolfClient, HostedWolfClient>();
            services.AddTransient<IHostedService>(x => (IHostedService)x.GetRequiredService<IWolfClient>());

            services.AddOptions();
            if (configureOptions != null)
                services.Configure(configureOptions);

            return new HostedWolfClientServiceBuilder(services);
        }



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
