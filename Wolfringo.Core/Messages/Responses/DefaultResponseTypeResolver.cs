using System;
using System.Collections.Generic;
using System.Linq;

namespace TehGM.Wolfringo.Messages.Responses
{
    public class DefaultResponseTypeResolver : IResponseTypeResolver
    {
        private static readonly Type _mappingAttributeType = typeof(ResponseTypeAttribute);
        private static readonly Type _baseResponseType = ResponseTypeAttribute.BaseResponseType;
        private static readonly Type _baseMessageType = typeof(IWolfMessage);

        private readonly IDictionary<Type, Type> _cachedMapping = new Dictionary<Type, Type>();

        public Type GetMessageResponseType(Type messageType, Type fallbackType = null)
        {
            // check cache first for quick short circuit
            if (_cachedMapping.TryGetValue(messageType, out Type result))
                return result;

            // verify passed params
            if (!_baseMessageType.IsAssignableFrom(messageType))
                throw new ArgumentException($"Message type must implement {_baseMessageType.FullName}", nameof(messageType));
            if (!_baseResponseType.IsAssignableFrom(fallbackType))
                throw new ArgumentException($"Response type must inherit from {_baseResponseType.FullName}", nameof(messageType));

            // check attributes
            // note: not using generics here just for compatibility with earlier .NET Framework versions
            if (messageType.GetCustomAttributes(_mappingAttributeType, true).FirstOrDefault() is ResponseTypeAttribute mappingAttr)
                result = mappingAttr.ResponseType;
            // if not set with attribute and fallback type is provided, let's use that one
            else if (fallbackType != null)
                result = fallbackType;
            // if neither attribute or fallback are found, use base type
            else
                result = _baseResponseType;

            // cache the result and then return
            _cachedMapping[messageType] = result;
            return result;
        }
    }
}
