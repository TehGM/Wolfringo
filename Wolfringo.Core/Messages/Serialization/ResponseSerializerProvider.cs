using System;

namespace TehGM.Wolfringo.Messages.Serialization
{
    /// <summary>Default response type to response serializer map.</summary>
    /// <remarks><para>This class contains all Wolfringo library default response mappings, and will be used by default clients
    /// if no other map is provided.</para>
    /// <para>This class can be easily extended without inheritance. To provide custom mappings, pass your own instance of <see cref="ResponseSerializerProviderOptions"/> via the constructor.</para></remarks>
    public class ResponseSerializerProvider : ISerializerProvider<Type, IResponseSerializer>
    {
        /// <inheritdoc/>
        public IResponseSerializer FallbackSerializer => this.Options.FallbackSerializer;
        protected ResponseSerializerProviderOptions Options { get; }

        /// <summary>Creates default response serializer map.</summary>
        /// <param name="fallbackSerializer">Serializer to use as fallback. If null, <see cref="DefaultResponseSerializer"/> will be used.</param>
        public ResponseSerializerProvider(ResponseSerializerProviderOptions options)
        {
            this.Options = options;
        }

        /// <summary>Creates default response serializer map with default options.</summary>
        public ResponseSerializerProvider() : this(new ResponseSerializerProviderOptions()) { }

        /// <inheritdoc/>
        public IResponseSerializer GetSerializer(Type key)
        {
            lock (this.Options)
            {
                this.Options.Serializers.TryGetValue(key, out IResponseSerializer result);
                return result;
            }
        }
    }
}
