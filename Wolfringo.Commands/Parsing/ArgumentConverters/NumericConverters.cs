using System;
using System.Globalization;
using System.Numerics;
using System.Reflection;

namespace TehGM.Wolfringo.Commands.Parsing.ArgumentConverters
{
    /// <summary>Argument converter for int.</summary>
    public class Int32Converter : IArgumentConverter
    {
        /// <inheritdoc/>
        public bool CanConvert(ParameterInfo parameter)
            => typeof(Int32) == parameter.ParameterType;

        /// <inheritdoc/>
        public object Convert(ParameterInfo parameter, string arg)
            => System.Convert.ToInt32(arg, CultureInfo.InvariantCulture);
    }

    /// <summary>Argument converter for uint.</summary>
    public class UInt32Converter : IArgumentConverter
    {
        /// <inheritdoc/>
        public bool CanConvert(ParameterInfo parameter)
            => typeof(UInt32) == parameter.ParameterType;

        /// <inheritdoc/>
        public object Convert(ParameterInfo parameter, string arg)
            => System.Convert.ToUInt32(arg, CultureInfo.InvariantCulture);
    }

    /// <summary>Argument converter for short.</summary>
    public class Int16Converter : IArgumentConverter
    {
        /// <inheritdoc/>
        public bool CanConvert(ParameterInfo parameter)
            => typeof(Int16) == parameter.ParameterType;

        /// <inheritdoc/>
        public object Convert(ParameterInfo parameter, string arg)
            => System.Convert.ToInt16(arg, CultureInfo.InvariantCulture);
    }

    /// <summary>Argument converter for ushort.</summary>
    public class UInt16Converter : IArgumentConverter
    {
        /// <inheritdoc/>
        public bool CanConvert(ParameterInfo parameter)
            => typeof(UInt16) == parameter.ParameterType;

        /// <inheritdoc/>
        public object Convert(ParameterInfo parameter, string arg)
            => System.Convert.ToUInt16(arg, CultureInfo.InvariantCulture);
    }

    /// <summary>Argument converter for long.</summary>
    public class Int64Converter : IArgumentConverter
    {
        /// <inheritdoc/>
        public bool CanConvert(ParameterInfo parameter)
            => typeof(Int64) == parameter.ParameterType;

        /// <inheritdoc/>
        public object Convert(ParameterInfo parameter, string arg)
            => System.Convert.ToInt64(arg, CultureInfo.InvariantCulture);
    }

    /// <summary>Argument converter for ulong.</summary>
    public class UInt64Converter : IArgumentConverter
    {
        /// <inheritdoc/>
        public bool CanConvert(ParameterInfo parameter)
            => typeof(UInt64) == parameter.ParameterType;

        /// <inheritdoc/>
        public object Convert(ParameterInfo parameter, string arg)
            => System.Convert.ToUInt64(arg, CultureInfo.InvariantCulture);
    }

    /// <summary>Argument converter for byte.</summary>
    public class ByteConverter : IArgumentConverter
    {
        /// <inheritdoc/>
        public bool CanConvert(ParameterInfo parameter)
            => typeof(Byte) == parameter.ParameterType;

        /// <inheritdoc/>
        public object Convert(ParameterInfo parameter, string arg)
            => System.Convert.ToByte(arg, CultureInfo.InvariantCulture);
    }

    /// <summary>Argument converter for sbyte.</summary>
    public class SByteConverter : IArgumentConverter
    {
        /// <inheritdoc/>
        public bool CanConvert(ParameterInfo parameter)
            => typeof(SByte) == parameter.ParameterType;

        /// <inheritdoc/>
        public object Convert(ParameterInfo parameter, string arg)
            => System.Convert.ToSByte(arg, CultureInfo.InvariantCulture);
    }

    /// <summary>Argument converter for float.</summary>
    public class SingleConverter : IArgumentConverter
    {
        /// <inheritdoc/>
        public bool CanConvert(ParameterInfo parameter)
            => typeof(Single) == parameter.ParameterType;

        /// <inheritdoc/>
        public object Convert(ParameterInfo parameter, string arg)
            => System.Convert.ToSingle(arg, CultureInfo.InvariantCulture);
    }

    /// <summary>Argument converter for decimal.</summary>
    public class DecimalConverter : IArgumentConverter
    {
        /// <inheritdoc/>
        public bool CanConvert(ParameterInfo parameter)
            => typeof(Decimal) == parameter.ParameterType;

        /// <inheritdoc/>
        public object Convert(ParameterInfo parameter, string arg)
            => System.Convert.ToDecimal(arg, CultureInfo.InvariantCulture);
    }

    /// <summary>Argument converter for double.</summary>
    public class DoubleConverter : IArgumentConverter
    {
        /// <inheritdoc/>
        public bool CanConvert(ParameterInfo parameter)
            => typeof(Double) == parameter.ParameterType;

        /// <inheritdoc/>
        public object Convert(ParameterInfo parameter, string arg)
            => System.Convert.ToDouble(arg, CultureInfo.InvariantCulture);
    }

    /// <summary>Argument converter for BigInteger.</summary>
    public class BigIntegerConverter : IArgumentConverter
    {
        /// <inheritdoc/>
        public bool CanConvert(ParameterInfo parameter)
            => typeof(BigInteger) == parameter.ParameterType;

        /// <inheritdoc/>
        public object Convert(ParameterInfo parameter, string arg)
            => BigInteger.Parse(arg, CultureInfo.InvariantCulture);
    }
}
