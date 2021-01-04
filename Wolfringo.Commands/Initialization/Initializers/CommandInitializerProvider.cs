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
        /// <summary>Options used by this provider.</summary>
        protected CommandInitializerProviderOptions Options { get; }
        /// <summary>Whether this provider will dispose initializers.</summary>
        /// <remarks><para>This will be set to true if <see cref="Options"/> weren't provided to the constructor.</para>
        /// <para>Disposing will happen when <see cref="Dispose"/> is called.</para></remarks>
        protected bool DisposeInitializers { get; }

        /// <summary>Creates default command initializer map.</summary>
        public CommandInitializerProvider(CommandInitializerProviderOptions options) : this(options, false) { }

        /// <summary>Creates default command initializer map with default options.</summary>
        public CommandInitializerProvider() : this(new CommandInitializerProviderOptions(), true) { }

        private CommandInitializerProvider(CommandInitializerProviderOptions options, bool disposeInitializers)
        {
            // validate options before assigning
            foreach (KeyValuePair<Type, ICommandInitializer> mapping in options.Initializers)
                ThrowIfInvalidCommandType(mapping.Key);

            this.Options = options;
            this.DisposeInitializers = disposeInitializers;
        }

        /// <inheritdoc/>
        public virtual ICommandInitializer GetInitializer(Type commandAttributeType)
        {
            ThrowIfInvalidCommandType(commandAttributeType);
            lock (this.Options)
            {
                this.Options.Initializers.TryGetValue(commandAttributeType, out ICommandInitializer result);
                return result;
            }
        }

        /// <summary>Throws an exception if given command attribute is of incorrect type.</summary>
        /// <param name="commandAttributeType">Type of command attribute.</param>
        /// <exception cref="ArgumentException"><paramref name="commandAttributeType"/> does not inherit from <see cref="CommandAttributeBase"/>.</exception>
        protected void ThrowIfInvalidCommandType(Type commandAttributeType)
        {
            if (!typeof(CommandAttributeBase).IsAssignableFrom(commandAttributeType))
                throw new ArgumentException($"Command attribute type must inherit from {typeof(CommandAttributeBase).Name}", nameof(commandAttributeType));
        }

        /// <summary>Disposes the map.</summary>
        /// <remarks>If any of the mapped initializers implements <see cref="IDisposable"/>, it'll also be disposed, unless options were provided via constructor from external source.</remarks>
        public virtual void Dispose()
        {
            if (!this.DisposeInitializers)
                return;

            IEnumerable<object> disposables;
            lock (this.Options)
            {
                disposables = this.Options.Initializers.Values.Where(c => c is IDisposable);
                this.Options.Initializers.Clear();
            }
            foreach (object disposable in disposables)
                try { (disposable as IDisposable)?.Dispose(); } catch { }
        }
    }
}
