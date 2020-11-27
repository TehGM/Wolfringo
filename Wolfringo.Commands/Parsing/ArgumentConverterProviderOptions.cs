using System;
using System.Collections.Generic;
using System.Numerics;
using TehGM.Wolfringo.Commands.Parsing.ArgumentConverters;

namespace TehGM.Wolfringo.Commands.Parsing
{
    /// <summary>Options for default command argument converter provider.</summary>
    /// <seealso cref="ArgumentConverterProvider"/>
    /// <see cref="IArgumentConverter"/>
    public class ArgumentConverterProviderOptions
    {
        /// <summary>Fallback converter used to convert any enum type that is not explicitly mapped.</summary>
        public IArgumentConverter EnumConverter { get; set; } = new EnumConverter();

        /// <summary>Map for parameter type and assigned argument converter.</summary>
        /// <remarks><para>Converters mapped by default:<br/>
        /// <see cref="string"/> - <see cref="StringConverter"/><br/>
        /// <see cref="char"/> - <see cref="CharConverter"/><br/>
        /// <see cref="bool"/> - <see cref="BooleanConverter"/><br/>
        /// <see cref="short"/> - <see cref="Int16Converter"/><br/>
        /// <see cref="ushort"/> - <see cref="UInt16Converter"/><br/>
        /// <see cref="int"/> - <see cref="Int32Converter"/><br/>
        /// <see cref="uint"/> - <see cref="UInt32Converter"/><br/>
        /// <see cref="long"/> - <see cref="Int64Converter"/><br/>
        /// <see cref="ulong"/> - <see cref="UInt64Converter"/><br/>
        /// <see cref="float"/> - <see cref="SingleConverter"/><br/>
        /// <see cref="double"/> - <see cref="DoubleConverter"/><br/>
        /// <see cref="decimal"/> - <see cref="DecimalConverter"/><br/>
        /// <see cref="BigInteger"/> - <see cref="BigIntegerConverter"/><br/>
        /// <see cref="TimeSpan"/> - <see cref="TimeSpanConverter"/><br/>
        /// <see cref="DateTime"/> - <see cref="DateTimeConverter"/><br/>
        /// <see cref="DateTimeOffset"/> - <see cref="DateTimeOffsetConverter"/><br/>
        /// <see cref="WolfTimestamp"/> - <see cref="WolfTimestampConverter"/></para></remarks>
        public IDictionary<Type, IArgumentConverter> Converters { get; set; } = new Dictionary<Type, IArgumentConverter>()
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
}
