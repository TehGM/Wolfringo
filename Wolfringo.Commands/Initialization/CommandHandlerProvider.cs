using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TehGM.Wolfringo.Commands.Initialization
{
    /// <inheritdoc>/>
    /// <remarks><para>This provider will keep persistent handlers in its own cache, and reuse them when applicable.</para>
    /// <para>The persistent handler instances that implement <see cref="IDisposable"/> will be automatically disposed when <see cref="Dispose"/> method is called.</para></remarks>
    public class CommandHandlerProvider : ICommandHandlerProvider, IDisposable
    {
        private readonly IServiceProvider _services;

        private readonly IDictionary<Type, CommandHandlerDescriptor> _knownHandlerDescriptors;
        private readonly IDictionary<Type, CommandHandlerProviderResult> _persistentHandlers;

        /// <summary>Creates a new provider instance.</summary>
        /// <param name="services">Service provider with services to use for constructor injection.</param>
        public CommandHandlerProvider(IServiceProvider services)
        {
            this._services = services;
        }

        /// <inheritdoc>/>
        public ICommandHandlerProviderResult GetCommandHandler(ICommandInstanceDescriptor descriptor)
        {
            Type handlerType = descriptor.GetHandlerType();

            // if not shared, try persistent
            if (_persistentHandlers.TryGetValue(handlerType, out CommandHandlerProviderResult handler))
                return handler;

            // if no persistent handler was found, we need to create a new one - check if descriptor is known yet
            if (!_knownHandlerDescriptors.TryGetValue(handlerType, out CommandHandlerDescriptor handlerDescriptor))
            {
                // if descriptor not cached, create new one
                // start with grabbing all constructors
                IEnumerable<ConstructorInfo> allConstructors = handlerType
                    .GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                // check if any of the constructors are specifically designated to be used by Commands System
                IEnumerable<ConstructorInfo> selectedConstructors = allConstructors
                    .Select(ctor => (constructor: ctor, attribute: ctor.GetCustomAttribute<CommandHandlerConstructorAttribute>(false)))
                    .Where(ctor => ctor.attribute != null)
                    .OrderByDescending(ctor => ctor.attribute.Priority)
                    .ThenByDescending(ctor => ctor.constructor.GetParameters().Length)
                    .Select(ctor => ctor.constructor);

                // if no explicitly-attributed constructor found, grab all that are public
                if (!selectedConstructors.Any())
                    selectedConstructors = allConstructors
                        .Where(ctor => ctor.IsPublic)
                        .OrderByDescending(ctor => ctor.GetParameters().Length);

                // try to resolve dependencies for each constructor. First one that can be resolved wins
                foreach (ConstructorInfo ctor in selectedConstructors)
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
            handler = new CommandHandlerProviderResult(handlerDescriptor, handlerDescriptor.CreateInstance());

            // if it's a persistent instance, store it
            if (handlerDescriptor.IsPersistent())
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

        /// <summary>Disposes the provider.</summary>
        /// <remarks>Any persistent handler that implements <see cref="IDisposable"/> will also be disposed.</remarks>
        public void Dispose()
        {
            IEnumerable<object> disposableHandlers = _persistentHandlers.Values.Where(handler => handler is IDisposable);
            _persistentHandlers.Clear();
            foreach (object handler in disposableHandlers)
                try { (handler as IDisposable).Dispose(); } catch { }
        }
    }
}
