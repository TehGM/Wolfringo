using System;

namespace TehGM.Wolfringo.Messages.Serialization.Internal
{
    /// <summary>Json converter for retrieving "id" property if the token is an object.</summary>
    public class EntityIdConverter : ValueOrPropertyConverter
    {
        /// <summary>Json converter for retrieving "id" property if the token is an object.</summary>
        public EntityIdConverter()
            : base("id") { }

        /// <inheritdoc/>
        public override bool CanConvert(Type objectType)
        {
            switch (Type.GetTypeCode(objectType))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    return true;
                default:
                    return false;
            }
        }
    }
}
