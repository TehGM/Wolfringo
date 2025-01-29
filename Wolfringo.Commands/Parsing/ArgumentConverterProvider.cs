using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace TehGM.Wolfringo.Commands.Parsing
{
    /// <inheritdoc/>
    /// <remarks><para>This default command argument converter provider is designed to match a type to a converter, and automatically handle enums.</para>
    /// <para>Besides enums, all converters simply match the type. If your custom converter uses complex logic in its <see cref="IArgumentConverter.CanConvert(ParameterInfo)"/> method, please create own provider class, or inherit from this class.</para></remarks>
    public class ArgumentConverterProvider : IArgumentConverterProvider, IDisposable
    {
        /// <summary>Options used by this provider.</summary>
        protected ArgumentConverterProviderOptions Options { get; }
        /// <summary>Whether this provider will dispose converters.</summary>
        /// <remarks><para>This will be set to true if <see cref="Options"/> weren't provided to the constructor.</para>
        /// <para>Disposing will happen when <see cref="Dispose"/> is called.</para></remarks>
        protected bool DisposeConverters { get; }
#if NET9_0_OR_GREATER
        private readonly Lock _lock = new Lock();
#else
        private readonly object _lock = new object();
#endif

        /// <summary>Creates default converter provider.</summary>
        public ArgumentConverterProvider(ArgumentConverterProviderOptions options) : this(options, false) { }

        /// <summary>Creates default converter provider with default options.</summary>
        public ArgumentConverterProvider() : this(new ArgumentConverterProviderOptions(), true) { }

        private ArgumentConverterProvider(ArgumentConverterProviderOptions options, bool disposeConverters)
        {
            this.Options = options;
            this.DisposeConverters = disposeConverters;
        }

        /// <inheritdoc/>
        public virtual IArgumentConverter GetConverter(ParameterInfo parameter)
        {
            if (this.Options.Converters.TryGetValue(parameter.ParameterType, out IArgumentConverter converter) && converter?.CanConvert(parameter) == true)
                return converter;
            if (parameter.ParameterType.IsEnum)
                return this.Options.EnumConverter;
            return null;
        }

        /// <summary>Disposes the provider.</summary>
        /// <remarks>If any of the mapped converters implements <see cref="IDisposable"/>, it'll also be disposed, unless options were provided via constructor from external source.</remarks>
        public virtual void Dispose()
        {
            if (!this.DisposeConverters)
                return;

            IEnumerable<IDisposable> disposables;
            lock (this._lock)
            {
                disposables = this.Options.Converters.Values.Where(c => c is IDisposable).Select(c => c as IDisposable);
                if (this.Options.EnumConverter is IDisposable disposableEnumConverter)
                    disposables = disposables.Union(new IDisposable[] { disposableEnumConverter });
                this.Options.Converters.Clear();
            }
            foreach (IDisposable disposable in disposables)
                try { disposable?.Dispose(); } catch { }
        }
    }
}
