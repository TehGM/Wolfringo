using System;
using Newtonsoft.Json;
using TehGM.Wolfringo.Messages.Serialization.Internal;

namespace TehGM.Wolfringo
{
    /// <summary>Represents a WOLF Timestamp.</summary>
    /// <remarks>This type is designed to provide easy and out-of-the-box conversions between WOLF Timestamp long format and DateTime.</remarks>
    [JsonConverter(typeof(WolfTimestampConverter))]
    public struct WolfTimestamp : IEquatable<WolfTimestamp>, IEquatable<DateTime>, IEquatable<long>, IComparable<WolfTimestamp>, IComparable<DateTime>, IComparable<long>
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
