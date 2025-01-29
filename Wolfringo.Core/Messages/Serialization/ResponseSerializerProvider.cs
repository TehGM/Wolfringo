using System;
using System.Threading;

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
        /// <summary>Instance of options used by this provider.</summary>
        protected ResponseSerializerProviderOptions Options { get; }
#if NET9_0_OR_GREATER
        private readonly Lock _lock = new Lock();
#else
        private readonly object _lock = new object();
#endif

        /// <summary>Creates default response serializer map.</summary>
        /// <param name="options">Instance of options to use with this provider.</param>
        public ResponseSerializerProvider(ResponseSerializerProviderOptions options)
        {
            this.Options = options;
        }

        /// <summary>Creates default response serializer map with default options.</summary>
        public ResponseSerializerProvider() : this(new ResponseSerializerProviderOptions()) { }

        /// <inheritdoc/>
        public IResponseSerializer GetSerializer(Type key)
        {
            lock (this._lock)
            {
                this.Options.Serializers.TryGetValue(key, out IResponseSerializer result);
                return result;
            }
        }
    }
}
