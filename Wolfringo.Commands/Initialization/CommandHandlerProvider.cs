using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TehGM.Wolfringo.Commands.Initialization
{
    public class CommandHandlerProvider : ICommandHandlerProvider
    {
        private readonly IServiceProvider _services;

        private readonly IDictionary<Type, CommandHandlerDescriptor> _knownHandlerDescriptors;
        private readonly IDictionary<Type, object> _persistentHandlers;

        public CommandHandlerProvider(IServiceProvider services)
        {
            this._services = services;
        }

        public object GetCommandHandler(CommandAttributeBase commandAttribute, Type handlerType)
        {
            // if not shared, try persistent
            if (_persistentHandlers.TryGetValue(handlerType, out object handler))
                return handler;

            // if not persistent handler was found, we need to create a new one - check if descriptor is known yet
            if (!_knownHandlerDescriptors.TryGetValue(handlerType, out CommandHandlerDescriptor handlerDescriptor))
            {
                // if descriptor not cached, create new one
                // start with sorting constructors by params count
                IEnumerable<ConstructorInfo> constructors = handlerType
                    .GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .OrderByDescending(ctor => ctor.GetParameters().Length);

                // try to resolve dependencies for each constructor. First one that can be resolved wins
                foreach (ConstructorInfo ctor in constructors)
                {
                    if (TryCreateHandlerDescriptor(ctor, out handlerDescriptor))
                    {
                        // cache found descriptor
                        _knownHandlerDescriptors.Add(handlerType, handlerDescriptor);
                        break;
                    }
                }
                // throw if we didn't find any constructor we can resolve
                throw new InvalidOperationException($"Cannot create descriptor for type {handlerType.FullName} - none of the constructors can have its dependencies resolved");
            }

            // now that we have a descriptor, let's create an instance
            handler = handlerDescriptor.CreateInstance();

            // if it's a persistent instance, store it
            _persistentHandlers.Add(handlerType, handler);

            // finally, return the fresh handler
            return handler;
        }

        private bool TryCreateHandlerDescriptor(ConstructorInfo constructor, out CommandHandlerDescriptor result)
        {
            result = null;
            ParameterInfo[] ctorParams = constructor.GetParameters();
            object[] paramsValues = new object[ctorParams.Length];
            foreach (ParameterInfo param in ctorParams)
            {
                object value = this._services.GetService(param.ParameterType);
                if (value == null)
                {
                    if (param.IsOptional)
                        value = param.HasDefaultValue ? param.DefaultValue : null;
                    else
                        return false;
                }
                paramsValues[param.Position] = value;
            }
            result = new CommandHandlerDescriptor(constructor, paramsValues);
            return true;
        }
    }
}
