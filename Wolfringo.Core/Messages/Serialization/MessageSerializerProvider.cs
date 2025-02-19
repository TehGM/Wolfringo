using System.Threading;

namespace TehGM.Wolfringo.Messages.Serialization
{
    /// <summary>Default message command to message serializer map.</summary>
    /// <remarks><para>This class contains all Wolfringo library default message mappings, and will be used by default clients
    /// if no other map is provided.</para>
    /// <para>This class can be easily extended without inheritance. To provide custom mappings, pass your own instance of <see cref="MessageSerializerProviderOptions"/> via the constructor.</para></remarks>
    public class MessageSerializerProvider : ISerializerProvider<string, IMessageSerializer>
    {
        /// <inheritdoc/>
        public IMessageSerializer FallbackSerializer => this.Options.FallbackSerializer;
        /// <summary>Instance of options used by this provider.</summary>
        protected MessageSerializerProviderOptions Options { get; }

        /// <summary>Create a new instance of default provider.</summary>
        /// <param name="options">Options to use with this provider.</param>
        public MessageSerializerProvider(MessageSerializerProviderOptions options)
        {
            this.Options = options;
        }

        /// <summary>Create a new instance of default provider with default options.</summary>
        public MessageSerializerProvider() : this(new MessageSerializerProviderOptions()) { }

        /// <inheritdoc/>
        public IMessageSerializer GetSerializer(string key)
        {
            this.Options.Serializers.TryGetValue(key, out IMessageSerializer result);
            return result;
        }
    }
}
