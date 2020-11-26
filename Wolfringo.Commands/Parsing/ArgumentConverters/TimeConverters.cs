using System;
using System.Globalization;
using System.Reflection;

namespace TehGM.Wolfringo.Commands.Parsing.ArgumentConverters
{
    /// <summary>Argument converter for TimeSpan.</summary>
    public class TimeSpanConverter : IArgumentConverter
    {
        /// <inheritdoc/>
        public bool CanConvert(ParameterInfo parameter)
            => typeof(TimeSpan) == parameter.ParameterType;

        /// <inheritdoc/>
        public object Convert(ParameterInfo parameter, string arg)
            => TimeSpan.Parse(arg, CultureInfo.InvariantCulture);
    }

    /// <summary>Argument converter for DateTime.</summary>
    public class DateTimeConverter : IArgumentConverter
    {
        /// <inheritdoc/>
        public bool CanConvert(ParameterInfo parameter)
            => typeof(DateTime) == parameter.ParameterType;

        /// <inheritdoc/>
        public object Convert(ParameterInfo parameter, string arg)
            => DateTime.Parse(arg, CultureInfo.InvariantCulture);
    }

    /// <summary>Argument converter for DateTimeOffset.</summary>
    public class DateTimeOffsetConverter : IArgumentConverter
    {
        /// <inheritdoc/>
        public bool CanConvert(ParameterInfo parameter)
            => typeof(DateTimeOffset) == parameter.ParameterType;

        /// <inheritdoc/>
        public object Convert(ParameterInfo parameter, string arg)
            => DateTimeOffset.Parse(arg, CultureInfo.InvariantCulture);
    }

    /// <summary>Argument converter for WolfTimestamp.</summary>
    /// <seealso cref="WolfTimestamp"/>
    public class WolfTimestampConverter : IArgumentConverter
    {
        /// <inheritdoc/>
        public bool CanConvert(ParameterInfo parameter)
            => typeof(WolfTimestamp) == parameter.ParameterType;

        /// <inheritdoc/>
        public object Convert(ParameterInfo parameter, string arg)
        {
            if (long.TryParse(arg, out long longValue))
                return new WolfTimestamp(longValue);
            else
                return new WolfTimestamp(DateTime.Parse(arg));
        }
    }
}
