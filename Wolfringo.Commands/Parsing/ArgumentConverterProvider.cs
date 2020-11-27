using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using TehGM.Wolfringo.Commands.Parsing.ArgumentConverters;

namespace TehGM.Wolfringo.Commands.Parsing
{
    /// <inheritdoc/>
    /// <remarks><para>This default command argument converter provider is designed to match a type to a converter, and automatically handle enums.</para>
    /// <para>Besides enums, all converters simply match the type. If your custom converter uses complex logic in its <see cref="IArgumentConverter.CanConvert(Type)"/> method, please create own provider class, or inherit from this class.</para></remarks>
    public class ArgumentConverterProvider : IArgumentConverterProvider
    {
        /// <summary>Map used for direct type to converter matching.</summary>
        protected IDictionary<Type, IArgumentConverter> Map { get; }
        protected IArgumentConverter EnumConverter { get; set; }

        /// <summary>Creates default converter provider.</summary>
        public ArgumentConverterProvider()
        {
            this.EnumConverter = new EnumConverter();
            this.Map = new Dictionary<Type, IArgumentConverter>()
            {
                { typeof(string), new StringConverter() },
                { typeof(char), new CharConverter() },
                { typeof(bool), new BooleanConverter() },
                // numerics
                { typeof(short), new Int16Converter() },
                { typeof(ushort), new UInt16Converter() },
                { typeof(int), new Int32Converter() },
                { typeof(uint), new UInt32Converter() },
                { typeof(long), new Int64Converter() },
                { typeof(ulong), new UInt64Converter() },
                { typeof(float), new SingleConverter() },
                { typeof(double), new DoubleConverter() },
                { typeof(decimal), new DecimalConverter() },
                { typeof(BigInteger), new BigIntegerConverter() },
                // time
                { typeof(TimeSpan), new TimeSpanConverter() },
                { typeof(DateTime), new DateTimeConverter() },
                { typeof(DateTimeOffset), new DateTimeOffsetConverter() },
                { typeof(WolfTimestamp), new WolfTimestampConverter() }
            };
        }

        /// <summary>Creates default command argument converter provider.</summary>
        /// <param name="additionalMappings">Additional mappings. Can overwrite default mappings.</param>
        public ArgumentConverterProvider(IEnumerable<KeyValuePair<Type, IArgumentConverter>> additionalMappings) : this()
        {
            foreach (var pair in additionalMappings)
                this.MapConverter(pair.Key, pair.Value);
        }

        /// <inheritdoc/>
        public virtual IArgumentConverter GetConverter(ParameterInfo parameter)
        {
            if (this.Map.TryGetValue(parameter.ParameterType, out IArgumentConverter converter) && converter.CanConvert(parameter))
                return converter;
            if (parameter.ParameterType.IsEnum)
                return this.EnumConverter;
            return null;
        }

        /// <summary>Maps an argument type to a converter.</summary>
        /// <remarks><para>Using this method, it is possible to override converter for specified enum.<br/>
        /// By default, <see cref="Map"/> is checked before attempting to parse enum. If a specific enum is mapped, it'll be found in map and use the converter before default enum converter is used.</para>
        /// <para>Providing a converter for an already mapped type will overwrite the mapped converter.</para></remarks>
        /// <param name="argumentType">Type of the parameter.</param>
        /// <param name="converter">Converter to use for that parameter type.</param>
        public virtual void MapConverter(Type argumentType, IArgumentConverter converter)
            => this.Map[argumentType] = converter;
    }
}
