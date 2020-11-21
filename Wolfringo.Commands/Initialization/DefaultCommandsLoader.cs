﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace TehGM.Wolfringo.Commands.Initialization
{
    public class DefaultCommandsLoader : ICommandsLoader
    {
        private readonly ILogger _log;
        private readonly ICommandInitializerMap _initializers;

        public DefaultCommandsLoader(ICommandInitializerMap initializers, ILogger log)
        {
            this._initializers = initializers;
            this._log = log;
        }

        public Task<IEnumerable<ICommandInstanceDescriptor>> LoadFromAssemblyAsync(Assembly assembly, CancellationToken cancellationToken = default)
        {
            List<ICommandInstanceDescriptor> results = new List<ICommandInstanceDescriptor>();
            _log?.LogTrace("Loading assembly {Name}", assembly.FullName);
            IEnumerable<TypeInfo> types = assembly.DefinedTypes.Where(t => !t.IsAbstract && !t.ContainsGenericParameters
                && !Attribute.IsDefined(t, typeof(CompilerGeneratedAttribute)) && Attribute.IsDefined(t, typeof(CommandHandlerAttribute), true));
            if (!types.Any())
            {
                _log?.LogWarning("Cannot initialize commands from assembly {Name} - no non-static non-abstract non-generic classes with {Attribute}", assembly.FullName, nameof(CommandHandlerAttribute));
                return NullTask();
            }
            foreach (TypeInfo type in types)
                results.AddRange(LoadFromTypeAsync(type).GetAwaiter().GetResult());
            return Task.FromResult<IEnumerable<ICommandInstanceDescriptor>>(results);
        }

        public Task<IEnumerable<ICommandInstanceDescriptor>> LoadFromTypeAsync(TypeInfo type, CancellationToken cancellationToken = default)
        {
            List<ICommandInstanceDescriptor> results = new List<ICommandInstanceDescriptor>();
            _log?.LogTrace("Loading handler {Handler}", type.FullName);
            IEnumerable<MethodInfo> methods = type.DeclaredMethods.Where(m => !m.IsStatic && !Attribute.IsDefined(m, typeof(CompilerGeneratedAttribute)) && Attribute.IsDefined(m, typeof(CommandAttributeBase), true));
            if (!methods.Any())
            {
                _log?.LogWarning("Cannot initialize commands from type {Handler} - no method with {Attribute}", type.FullName, nameof(CommandAttributeBase));
                return NullTask();
            }
            foreach (MethodInfo method in methods)
                results.AddRange(LoadFromMethodAsync(method).GetAwaiter().GetResult());
            return Task.FromResult<IEnumerable<ICommandInstanceDescriptor>>(results);
        }

        public Task<IEnumerable<ICommandInstanceDescriptor>> LoadFromMethodAsync(MethodInfo method, CancellationToken cancellationToken = default)
        {
            List<ICommandInstanceDescriptor> results = new List<ICommandInstanceDescriptor>();
            _log?.LogTrace("Loading command {Name}", method.Name);
            IEnumerable<CommandAttributeBase> attributes = method.GetCustomAttributes<CommandAttributeBase>(true);
            if (!attributes.Any())
            {
                _log?.LogWarning("Cannot initialize command from {Handler}'s method {Name} - {Attribute} missing", method.DeclaringType.FullName, method.Name, nameof(CommandAttributeBase));
                return NullTask();
            }
            foreach (CommandAttributeBase attribute in attributes)
            {
                // ensure there's a valid initializer
                ICommandInitializer initializer = _initializers.GetMappedInitializer(attribute.GetType());
                if (initializer == null)
                    throw new InvalidOperationException($"No initializer found for command type {attribute.GetType().Name}");

                // add the command
                CommandInstanceDescriptor descriptor = new CommandInstanceDescriptor(attribute, method);
                results.Add(new CommandInstanceDescriptor(attribute, method));
                _log?.LogTrace("Command {Name} from handler {Handler} loaded", method.Name, method.DeclaringType.Name);
            }
            return Task.FromResult<IEnumerable<ICommandInstanceDescriptor>>(results);
        }

        private static Task<IEnumerable<ICommandInstanceDescriptor>> NullTask()
            => Task.FromResult<IEnumerable<ICommandInstanceDescriptor>>(null);
    }
}