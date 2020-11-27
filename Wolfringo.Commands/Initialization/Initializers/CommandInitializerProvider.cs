using System;
using System.Collections.Generic;
using System.Linq;
using TehGM.Wolfringo.Commands.Attributes;

namespace TehGM.Wolfringo.Commands.Initialization
{
    /// <inheritdoc/>
    /// <remarks>This is a default initializer map, and contains all initializers provided with the Wolfringo.Commands library by default.</remarks>
    public class CommandInitializerProvider : ICommandInitializerProvider, IDisposable
    {
        private CommandInitializerProviderOptions _options;
        private bool _disposeInitializers;

        /// <summary>Creates default command initializer map.</summary>
        public CommandInitializerProvider(CommandInitializerProviderOptions options) : this(options, false) { }

        /// <summary>Creates default command initializer map with default options.</summary>
        public CommandInitializerProvider() : this(new CommandInitializerProviderOptions(), true) { }

        private CommandInitializerProvider(CommandInitializerProviderOptions options, bool disposeInitializers)
        {
            // validate options before assigning
            foreach (KeyValuePair<Type, ICommandInitializer> mapping in options.Initializers)
                ThrowIfInvalidCommandType(mapping.Key);

            this._options = options;
            this._disposeInitializers = disposeInitializers;
        }

        /// <inheritdoc/>
        public ICommandInitializer GetInitializer(Type commandAttributeType)
        {
            ThrowIfInvalidCommandType(commandAttributeType);
            lock (_options)
            {
                this._options.Initializers.TryGetValue(commandAttributeType, out ICommandInitializer result);
                return result;
            }
        }

        private void ThrowIfInvalidCommandType(Type commandAttributeType)
        {
            if (!typeof(CommandAttributeBase).IsAssignableFrom(commandAttributeType))
                throw new ArgumentException($"Command attribute type must inherit from {typeof(CommandAttributeBase).Name}", nameof(commandAttributeType));
        }

        /// <summary>Disposes the map.</summary>
        /// <remarks>If any of the mapped initializers implements <see cref="IDisposable"/>, it'll also be disposed, unless options were provided via constructor from external source.</remarks>
        public void Dispose()
        {
            if (!this._disposeInitializers)
                return;

            IEnumerable<object> disposables;
            lock (_options)
            {
                disposables = _options.Initializers.Values.Where(c => c is IDisposable);
                _options.Initializers.Clear();
            }
            foreach (object disposable in disposables)
                try { (disposable as IDisposable)?.Dispose(); } catch { }
        }
    }
}
