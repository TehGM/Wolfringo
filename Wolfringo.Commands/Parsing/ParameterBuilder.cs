using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace TehGM.Wolfringo.Commands.Parsing
{
    public static class ParameterBuilder
    {
        public static object[] BuildParamsAsync(IEnumerable<ParameterInfo> parameters, ParameterBuilderValues values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));
            if (parameters?.Any() != true)
                return Array.Empty<object>();

            object[] paramsValues = new object[parameters.Count()];
            int argIndex = 0; // parse arg index, so they're handled in order

            foreach (ParameterInfo param in parameters)
            {
                // check additionals first, in case they override something
                if (TryFindAdditional(param.ParameterType, values.AdditionalObjects, out object value)) { }
                // from context
                else if (values.Context != null && param.ParameterType.IsAssignableFrom(values.Context.GetType()))
                    value = values.Context;
                else if (values.Context != null && param.ParameterType.IsAssignableFrom(values.Context.Message.GetType()))
                    value = values.Context.Message;
                else if (values.Context != null && param.ParameterType.IsAssignableFrom(values.Context.Client.GetType()))
                    value = values.Context.Client;
                // cancellation token
                else if (param.ParameterType.IsAssignableFrom(typeof(CancellationToken)))
                    value = values.CancellationToken;
                // from services
                else if (TryGetService(param.ParameterType, values.Services, out value)) { }
                // from args
                else
                {
                    // try to convert as arg
                    if (TryConvertArgument(param, argIndex, values, out value))
                    {
                        argIndex++;
                    }
                    // if it's optional, just let it pass
                    else if (param.IsOptional)
                        value = param.HasDefaultValue ? param.DefaultValue : null;
                    // if not default and not thrown conversion error, but still not found yet - means it's arg that is expected, but user didn't provide it in command - so throw
                    else if (argIndex < values.Args.Length)
                        throw new InvalidOperationException($"User did not provide argument {param.Name}");
                    // none found, throw
                    else
                        throw new InvalidOperationException($"Unsupported param type: {param.ParameterType.FullName}");
                }
                paramsValues[param.Position] = value;
            }

            return paramsValues;
        }

        private static bool TryFindAdditional(Type type, IEnumerable<object> additionals, out object result)
        {
            if (additionals?.Any() == true)
            {
                foreach (object obj in additionals)
                {
                    if (type.IsAssignableFrom(obj.GetType()))
                    {
                        result = obj;
                        return true;
                    }
                }
            }

            result = null;
            return false;
        }

        private static bool TryGetService(Type type, IServiceProvider services, out object result)
        {
            if (services == null)
            {
                result = null;
                return false;
            }
            result = services.GetService(type);
            return result != null;
        }

        private static bool TryConvertArgument(ParameterInfo parameter, int argIndex, ParameterBuilderValues values, out object result)
        {
            if (argIndex < 0)
                throw new ArgumentException("Argument index cannot be negative", nameof(argIndex));
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            // check if we didn't run out of args
            if (values.Args == null || values.Args.Length - 1 < argIndex)
            {
                result = null;
                return false;
            }

            // get from provider
            IArgumentConverterProvider provider = values.ArgumentConverterProvider;
            IArgumentConverter converter = provider?.GetConverter(parameter);
            if (converter == null)
            {
                result = null;
                return false;
            }

            result = converter.Convert(parameter, values.Args[argIndex]);
            return true;
        }
    }

    public class ParameterBuilderValues
    {
        public string[] Args { get; set; }
        public ICommandContext Context { get; set; }
        public IServiceProvider Services { get; set; }
        public IArgumentConverterProvider ArgumentConverterProvider { get; set; }
        public CancellationToken CancellationToken { get; set; }
        public IEnumerable<object> AdditionalObjects { get; set; }
    }
}
