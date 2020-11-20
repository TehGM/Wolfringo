using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TehGM.Wolfringo.Commands.Initialization
{
    public static class CommandInitializerHelper
    {
        public static bool TryCreateHandlerDescriptor(this ConstructorInfo constructor, IWolfClient client, IServiceProvider services, out HandlerDescriptor result)
        {
            result = null;
            ParameterInfo[] ctorParams = constructor.GetParameters();
            object[] paramsValues = new object[ctorParams.Length];
            foreach (ParameterInfo param in ctorParams)
            {
                object value;
                if (param.ParameterType.IsAssignableFrom(client.GetType()))
                    value = client;
                else
                {
                    value = services.GetService(param.ParameterType);
                    if (value == null)
                    {
                        if (param.IsOptional)
                            value = param.HasDefaultValue ? param.DefaultValue : null;
                        else
                            return false;
                    }
                }
                paramsValues[param.Position] = value;
            }
            result = new HandlerDescriptor(constructor, paramsValues);
            return true;
        }

        public static HandlerDescriptor FindHandlerDescriptor(this Type handlerType, IWolfClient client, IServiceProvider services)
        {
            IEnumerable<ConstructorInfo> constructors = handlerType
                .GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .OrderByDescending(ctor => ctor.GetParameters().Length);
            foreach (ConstructorInfo ctor in constructors)
            {
                if (ctor.TryCreateHandlerDescriptor(client, services, out HandlerDescriptor result))
                    return result;
            }
            throw new InvalidOperationException($"Cannot create descriptor for type {handlerType.FullName} - none of the constructors can have its dependencies resolved");
        }
    }
}
