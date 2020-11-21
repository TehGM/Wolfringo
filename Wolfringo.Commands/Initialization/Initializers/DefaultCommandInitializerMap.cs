using System;
using System.Collections.Generic;
using System.Linq;

namespace TehGM.Wolfringo.Commands.Initialization
{
    public class DefaultCommandInitializerMap : ICommandInitializerMap, IDisposable
    {
        private IDictionary<Type, ICommandInitializer> _map;

        /// <summary>Creates default message serializer map.</summary>
        /// <param name="fallbackSerializer">Serializer to use as fallback. If null, 
        /// <see cref="DefaultMessageSerializer{T}"/> for <see cref="IWolfMessage"/> will be used.</param>
        public DefaultCommandInitializerMap()
        {
            this._map = new Dictionary<Type, ICommandInitializer>()
            {
                { typeof(RegexCommandAttribute), new RegexCommandInitializer() }
            };
        }

        /// <summary>Creates default message serializer map.</summary>
        /// <param name="additionalMappings">Additional mappings. Can overwrite default mappings.</param>
        /// <param name="fallbackSerializer">Serializer to use as fallback. If null, 
        /// <see cref="DefaultMessageSerializer{T}"/> for <see cref="IWolfMessage"/> will be used.</param>
        public DefaultCommandInitializerMap(IEnumerable<KeyValuePair<Type, ICommandInitializer>> additionalMappings) : this()
        {
            foreach (var pair in additionalMappings)
                this.MapInitializer(pair.Key, pair.Value);
        }

        public ICommandInitializer GetMappedInitializer(Type commandAttributeType)
        {
            this._map.TryGetValue(commandAttributeType, out ICommandInitializer result);
            return result;
        }

        public void MapInitializer(Type commandAttributeType, ICommandInitializer initializer)
        {
            if (!typeof(CommandAttributeBase).IsAssignableFrom(commandAttributeType))
                throw new ArgumentException($"Command attribute type must inherit from {typeof(CommandAttributeBase).Name}", nameof(commandAttributeType));
            this._map[commandAttributeType] = initializer;
        }

        public void Dispose()
        {
            IEnumerable<object> disposables = _map.Values.Where(c => c is IDisposable);
            _map.Clear();
            foreach (object disposable in disposables)
                try { (disposable as IDisposable)?.Dispose(); } catch { }
        }
    }
}
