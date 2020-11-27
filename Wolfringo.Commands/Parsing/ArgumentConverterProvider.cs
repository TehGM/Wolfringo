using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TehGM.Wolfringo.Commands.Parsing
{
    /// <inheritdoc/>
    /// <remarks><para>This default command argument converter provider is designed to match a type to a converter, and automatically handle enums.</para>
    /// <para>Besides enums, all converters simply match the type. If your custom converter uses complex logic in its <see cref="IArgumentConverter.CanConvert(Type)"/> method, please create own provider class, or inherit from this class.</para></remarks>
    public class ArgumentConverterProvider : IArgumentConverterProvider, IDisposable
    {
        private ArgumentConverterProviderOptions _options;
        private bool _disposeConverters;

        /// <summary>Creates default converter provider.</summary>
        public ArgumentConverterProvider(ArgumentConverterProviderOptions options) : this(options, false) { }

        /// <summary>Creates default converter provider with default options.</summary>
        public ArgumentConverterProvider() : this(new ArgumentConverterProviderOptions(), true) { }

        private ArgumentConverterProvider(ArgumentConverterProviderOptions options, bool disposeConverters)
        {
            this._options = options;
            this._disposeConverters = disposeConverters;
        }

        /// <inheritdoc/>
        public virtual IArgumentConverter GetConverter(ParameterInfo parameter)
        {
            if (this._options.Converters.TryGetValue(parameter.ParameterType, out IArgumentConverter converter) && converter.CanConvert(parameter))
                return converter;
            if (parameter.ParameterType.IsEnum)
                return this._options.EnumConverter;
            return null;
        }

        /// <summary>Disposes the provider.</summary>
        /// <remarks>If any of the mapped converters implements <see cref="IDisposable"/>, it'll also be disposed, unless options were provided via constructor from external source.</remarks>
        public void Dispose()
        {
            if (!this._disposeConverters)
                return;

            IEnumerable<IDisposable> disposables;
            lock (_options)
            {
                disposables = _options.Converters.Values.Where(c => c is IDisposable).Select(c => c as IDisposable);
                if (_options.EnumConverter is IDisposable disposableEnumConverter)
                    disposables = disposables.Union(new IDisposable[] { disposableEnumConverter });
                _options.Converters.Clear();
            }
            foreach (object disposable in disposables)
                try { (disposable as IDisposable)?.Dispose(); } catch { }
        }
    }
}
