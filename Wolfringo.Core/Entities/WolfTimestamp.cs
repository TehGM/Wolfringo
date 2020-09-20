using System;
using System.ComponentModel;
using Newtonsoft.Json;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo
{
    /// <summary>Represents a WOLF Timestamp.</summary>
    /// <remarks>This type is designed to provide easy and out-of-the-box conversions between WOLF Timestamp long format and DateTime.</remarks>
    [JsonConverter(typeof(WolfTimestampConverter))]
    [TypeConverter(typeof(WolfTimestampTypeConverter))]
    public struct WolfTimestamp : IEquatable<WolfTimestamp>, IEquatable<DateTime>, IEquatable<long>, IComparable<WolfTimestamp>, IComparable<DateTime>, IComparable<long>, IConvertible
    {
        /// <summary>Unix Epoch.</summary>
        public static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private readonly long _value;

        /// <summary>Creates a new WOLF timestamp equal to provided value.</summary>
        /// <param name="value">Timestamp value.</param>
        public WolfTimestamp(long value)
        {
            this._value = value;
        }

        /// <summary>Creates a new WOLF timestamp equal to provided DateTime.</summary>
        /// <param name="value">DateTime of timestamp.</param>
        public WolfTimestamp(DateTime value)
        {
            long ticks = (value - Epoch).Ticks;
            this._value = ticks / 10;
        }

        #region Conversion
        /// <summary>Gets DateTime that equals this WOLF timestamp.</summary>
        /// <returns>DateTime representation of the timestamp.</returns>
        public DateTime ToDateTime()
            => Epoch.AddTicks(_value * 10);

        public static implicit operator DateTime(WolfTimestamp timestamp)
            => timestamp.ToDateTime();
        public static implicit operator WolfTimestamp(DateTime timestamp)
            => new WolfTimestamp(timestamp);

        public static implicit operator long(WolfTimestamp timestamp)
            => timestamp._value;
        public static implicit operator WolfTimestamp(long timestamp)
            => new WolfTimestamp(timestamp);

        /// <inheritdoc/>
        public override string ToString()
            => this._value.ToString();

        /// <inheritdoc/>
        TypeCode IConvertible.GetTypeCode()
            => TypeCode.Int64;
        /// <inheritdoc/>
        DateTime IConvertible.ToDateTime(IFormatProvider provider)
            => this.ToDateTime();
        /// <inheritdoc/>
        decimal IConvertible.ToDecimal(IFormatProvider provider)
            => this._value;
        /// <inheritdoc/>
        double IConvertible.ToDouble(IFormatProvider provider)
            => this._value;
        /// <inheritdoc/>
        float IConvertible.ToSingle(IFormatProvider provider)
            => this._value;
        /// <inheritdoc/>
        string IConvertible.ToString(IFormatProvider provider)
            => this.ToString();
        /// <inheritdoc/>
        long IConvertible.ToInt64(IFormatProvider provider)
            => this._value;
        /// <inheritdoc/>
        ulong IConvertible.ToUInt64(IFormatProvider provider)
            => (ulong)this._value;
        /// <inheritdoc/>
        bool IConvertible.ToBoolean(IFormatProvider provider)
            => throw new InvalidCastException($"Cannot cast {this.GetType().FullName} to {typeof(Boolean).FullName}");
        /// <inheritdoc/>
        byte IConvertible.ToByte(IFormatProvider provider)
            => throw new InvalidCastException($"Cannot cast {this.GetType().FullName} to {typeof(Byte).FullName}");
        /// <inheritdoc/>
        char IConvertible.ToChar(IFormatProvider provider)
            => throw new InvalidCastException($"Cannot cast {this.GetType().FullName} to {typeof(Char).FullName}");
        /// <inheritdoc/>
        short IConvertible.ToInt16(IFormatProvider provider)
            => throw new InvalidCastException($"Cannot cast {this.GetType().FullName} to {typeof(Int16).FullName}");
        /// <inheritdoc/>
        int IConvertible.ToInt32(IFormatProvider provider)
            => throw new InvalidCastException($"Cannot cast {this.GetType().FullName} to {typeof(Int32).FullName}");
        /// <inheritdoc/>
        sbyte IConvertible.ToSByte(IFormatProvider provider)
            => throw new InvalidCastException($"Cannot cast {this.GetType().FullName} to {typeof(SByte).FullName}");
        /// <inheritdoc/>
        ushort IConvertible.ToUInt16(IFormatProvider provider)
            => throw new InvalidCastException($"Cannot cast {this.GetType().FullName} to {typeof(UInt16).FullName}");
        /// <inheritdoc/>
        uint IConvertible.ToUInt32(IFormatProvider provider)
            => throw new InvalidCastException($"Cannot cast {this.GetType().FullName} to {typeof(UInt32).FullName}");

        /// <inheritdoc/>
        object IConvertible.ToType(Type conversionType, IFormatProvider provider)
        {
            if (conversionType.IsAssignableFrom(this.GetType()))
                return this;
            return Convert.ChangeType(this._value, conversionType);
        }
        #endregion

        #region Equality checks
        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj is long objLong)
                return this.Equals(objLong);
            if (obj is DateTime objDateTime)
                return this.Equals(objDateTime);
            if (obj is WolfTimestamp objTimestamp)
                return this.Equals(objTimestamp);
            return false;
        }

        /// <inheritdoc/>
        public bool Equals(WolfTimestamp other)
            => this._value.Equals(other._value);

        /// <inheritdoc/>
        public bool Equals(long other)
            => this._value.Equals(other);

        /// <inheritdoc/>
        public bool Equals(DateTime other)
            => this.ToDateTime().Equals(other);

        /// <inheritdoc/>
        public override int GetHashCode()
            => -1939223833 + this._value.GetHashCode();

        public static bool operator ==(WolfTimestamp left, WolfTimestamp right)
            => left.Equals(right);

        public static bool operator !=(WolfTimestamp left, WolfTimestamp right)
            => !(left == right);
        #endregion

        #region Comparison checks
        /// <inheritdoc/>
        public int CompareTo(long other)
            => this._value.CompareTo(other);

        /// <inheritdoc/>
        public int CompareTo(DateTime other)
            => this.ToDateTime().CompareTo(other);

        /// <inheritdoc/>
        public int CompareTo(WolfTimestamp other)
            => this._value.CompareTo(other._value);
        #endregion
    }
}
