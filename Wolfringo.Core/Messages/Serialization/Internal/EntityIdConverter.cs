using System;

namespace TehGM.Wolfringo.Messages.Serialization.Internal
{
    public class EntityIdConverter : ValueOrPropertyConverter
    {
        public EntityIdConverter()
            : base("id") { }

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
