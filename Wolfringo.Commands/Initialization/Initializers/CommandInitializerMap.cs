using System;
using System.Collections.Generic;
using System.Linq;
using TehGM.Wolfringo.Commands.Attributes;

namespace TehGM.Wolfringo.Commands.Initialization
{
    /// <inheritdoc/>
    /// <remarks>This is a default initializer map, and contains all initializers provided with the Wolfringo.Commands library by default.</remarks>
    public class CommandInitializerMap : ICommandInitializerMap, IDisposable
    {
        private IDictionary<Type, ICommandInitializer> _map;

        /// <summary>Creates default command initializer map.</summary>
        public CommandInitializerMap()
        {
            this._map = new Dictionary<Type, ICommandInitializer>()
            {
                { typeof(RegexCommandAttribute), new RegexCommandInitializer() },
                { typeof(CommandAttribute), new StandardCommandInitializer() }
            };
        }

        /// <summary>Creates default command initializer map.</summary>
        /// <param name="additionalMappings">Additional mappings. Can overwrite default mappings.</param>
        public CommandInitializerMap(IEnumerable<KeyValuePair<Type, ICommandInitializer>> additionalMappings) : this()
        {
            foreach (var pair in additionalMappings)
                this.MapInitializer(pair.Key, pair.Value);
        }

        /// <inheritdoc/>
        public ICommandInitializer GetMappedInitializer(Type commandAttributeType)
        {
            lock (_map)
            {
                this._map.TryGetValue(commandAttributeType, out ICommandInitializer result);
                return result;
            }
        }

        /// <inheritdoc/>
        /// <remarks><para>If the <paramref name="commandAttributeType"/> already has a mapped serializer, it'll be replaced.</para>
        /// <paramref name="commandAttributeType"/> must be a type inheriting from <see cref="CommandAttributeBase"/>.</remarks>
        /// <exception cref="ArgumentException">Provided command attribute type does not inherit from <see cref="CommandAttributeBase"/>.</exception>
        public void MapInitializer(Type commandAttributeType, ICommandInitializer initializer)
        {
            if (!typeof(CommandAttributeBase).IsAssignableFrom(commandAttributeType))
                throw new ArgumentException($"Command attribute type must inherit from {typeof(CommandAttributeBase).Name}", nameof(commandAttributeType));
            lock (_map)
                this._map[commandAttributeType] = initializer;
        }

        /// <summary>Disposes the map.</summary>
        /// <remarks>If any of the mapped initializers implements <see cref="IDisposable"/>, it'll also be disposed.</remarks>
        public void Dispose()
        {
            IEnumerable<object> disposables;
            lock (_map)
            {
                disposables = _map.Values.Where(c => c is IDisposable);
                _map.Clear();
            }
            foreach (object disposable in disposables)
                try { (disposable as IDisposable)?.Dispose(); } catch { }
        }
    }
}
