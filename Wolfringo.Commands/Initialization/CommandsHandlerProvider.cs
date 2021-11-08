using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TehGM.Wolfringo.Commands.Parsing;

namespace TehGM.Wolfringo.Commands.Initialization
{
    /// <inheritdoc/>
    /// <remarks><para>This provider will keep persistent handlers in its own cache, and reuse them when applicable.</para>
    /// <para>The persistent handler instances that implement <see cref="IDisposable"/> will be automatically disposed when <see cref="Dispose"/> method is called.</para></remarks>
    public class CommandsHandlerProvider : ICommandsHandlerProvider, IDisposable
    {
        private readonly IDictionary<Type, ConstructorInfo> _knownConstructors;
        private readonly IDictionary<Type, CommandsHandlerProviderResult> _persistentHandlers;
        private readonly object _lock;

        /// <summary>Creates a new provider instance.</summary>
        public CommandsHandlerProvider()
        {
            this._knownConstructors = new Dictionary<Type, ConstructorInfo>();
            this._persistentHandlers = new Dictionary<Type, CommandsHandlerProviderResult>();
            this._lock = new object();
        }

        /// <inheritdoc/>
        public ICommandsHandlerProviderResult GetCommandHandler(ICommandInstanceDescriptor descriptor, IServiceProvider services)
        {
            Type handlerType = descriptor.GetHandlerType();
            CommandHandlerDescriptor handlerDescriptor = null;

            lock (_lock)
            {
                // check if persistent
                if (_persistentHandlers.TryGetValue(handlerType, out CommandsHandlerProviderResult handler))
                    return handler;

                // keep resolved services to avoid recreating them in case of multiple constructors
                IDictionary<Type, object> resolvedServices = new Dictionary<Type, object>();

                // if no persistent handler was found, we need to create a new one - check if constructor is known yet
                if (!_knownConstructors.TryGetValue(handlerType, out ConstructorInfo handlerConstructor))
                {
                    // if descriptor not cached, create new one
                    // start with grabbing all constructors
                    IEnumerable<ConstructorInfo> allConstructors = handlerType
                        .GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                    // check if any of the constructors are specifically designated to be used by Commands System
                    IEnumerable<ConstructorInfo> selectedConstructors = allConstructors
                        .Select(ctor => (constructor: ctor, attribute: ctor.GetCustomAttribute<CommandsHandlerConstructorAttribute>(false)))
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
                        if (TryCreateHandlerDescriptor(ctor, services, out handlerDescriptor, ref resolvedServices))
                        {
                            // cache found descriptor
                            _knownConstructors.Add(handlerType, ctor);
                            handlerConstructor = ctor;
                            break;
                        }
                    }
                    // throw if we didn't find any constructor we can resolve
                    if (handlerDescriptor == null)
                        throw new InvalidOperationException($"Cannot create descriptor for type {handlerType.FullName} - none of the constructors can have its dependencies resolved");
                }
                // if constructor is already known, just create a descriptor
                else
                    TryCreateHandlerDescriptor(handlerConstructor, services, out handlerDescriptor, ref resolvedServices);

                // now that we have a descriptor, let's create an instance
                handler = new CommandsHandlerProviderResult(handlerDescriptor, handlerDescriptor.CreateInstance());

                // if it's a persistent instance, store it
                if (handlerDescriptor.IsPersistent())
                    _persistentHandlers.Add(handlerType, handler);

                // finally, return the fresh handler
                return handler;
            }
        }

        private bool TryCreateHandlerDescriptor(ConstructorInfo constructor, IServiceProvider services, out CommandHandlerDescriptor result, ref IDictionary<Type, object> resolvedServices)
        {
            result = null;
            ParameterInfo[] ctorParams = constructor.GetParameters();
            object[] paramsValues = new object[ctorParams.Length];
            foreach (ParameterInfo param in ctorParams)
            {
                if (!resolvedServices.TryGetValue(param.ParameterType, out object value))
                {
                    if (ParameterBuilder.TryGetService(param.ParameterType, services, out value)) { }
                    else if (ParameterBuilder.TryGetGenericLogger(param.ParameterType, services, out value)) { }
                    else if (param.HasDefaultValue)
                        value = param.DefaultValue ?? null;
                    else if (param.IsOptional)
                        value = Type.Missing;
                    else
                        return false;

                    resolvedServices[param.ParameterType] = value;
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
            IEnumerable<object> disposableHandlers;
            lock (_lock)
            {
                disposableHandlers = _persistentHandlers.Values.Where(handler => handler is IDisposable);
                this._knownConstructors.Clear();
                this._persistentHandlers.Clear();
            }
            foreach (object handler in disposableHandlers)
                try { (handler as IDisposable).Dispose(); } catch { }
        }
    }
}
