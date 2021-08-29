using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TehGM.Wolfringo.Commands.Results;

namespace TehGM.Wolfringo.Commands.Parsing
{
    /// <inheritdoc/>
    public class ParameterBuilder : IParameterBuilder
    {
        /// <inheritdoc/>
        public async Task<ParameterBuildingResult> BuildParamsAsync(IEnumerable<ParameterInfo> parameters, ParameterBuilderValues values, CancellationToken cancellationToken = default)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));
            if (parameters?.Any() != true)
                return ParameterBuildingResult.Success(Array.Empty<object>());

            object[] paramsValues = new object[parameters.Count()];
            int argIndex = 0; // parse arg index, so they're handled in order

            foreach (ParameterInfo param in parameters)
            {
                cancellationToken.ThrowIfCancellationRequested();

                // check additionals first, in case they override something
                if (TryFindAdditional(param.ParameterType, values.AdditionalObjects, out object value)) { }
                // from context
                else if (values.Context != null && param.ParameterType.IsAssignableFrom(values.Context.GetType()))
                    value = values.Context;
                else if (values.Context != null && param.ParameterType.IsAssignableFrom(values.Context.Message.GetType()))
                    value = values.Context.Message;
                else if (values.Context != null && param.ParameterType.IsAssignableFrom(values.Context.Client.GetType()))
                    value = values.Context.Client;
                // command instance
                else if (values.CommandInstance != null && param.ParameterType.IsAssignableFrom(values.CommandInstance.GetType()))
                    value = values.CommandInstance;
                // cancellation token
                else if (param.ParameterType.IsAssignableFrom(typeof(CancellationToken)))
                    value = values.CancellationToken;
                // from services
                else if (TryGetService(param.ParameterType, values.Services, out value)) { }
                // logger from factory
                else if (TryGetGenericLogger(param.ParameterType, values.Services, out value)) { }
                // from args
                else
                {
                    // try to convert as arg
                    if (TryConvertArgument(param, argIndex, values, out value, out Exception convertingError))
                    {
                        argIndex++;
                    }
                    // if there's an error, let's return result with message - but without exception, as we don't want input errors to be logged
                    else if (convertingError != null)
                        return ParameterBuildingResult.Failure(new string[] {
                            await param.GetConvertingErrorAttribute().ToStringAsync(values.Context, values.Args[argIndex], param, cancellationToken).ConfigureAwait(false) });
                    // if it's optional, just let it pass
                    else if (param.IsOptional)
                        value = param.HasDefaultValue ? param.DefaultValue : null;
                    // if not default and not thrown conversion error, but still not found yet - means it's arg that is expected, but user didn't provide it in command - so return error with message - do not provide exception, as we don't want it logged
                    else if (argIndex <= values.Args.Length)
                        return ParameterBuildingResult.Failure(new string[] {
                            await param.GetMissingErrorAttribute().ToStringAsync(values.Context,
                            values.Args.Length > argIndex ? values.Args[argIndex] : string.Empty,
                            param, cancellationToken).ConfigureAwait(false) });
                    // none found, throw
                    else
                        throw new InvalidOperationException($"Unsupported param type: {param.ParameterType.FullName}");
                }
                paramsValues[param.Position] = value;
            }

            return ParameterBuildingResult.Success(paramsValues);
        }

        /// <summary>Attempts to find an object of specific type from enumerable of objects.</summary>
        /// <param name="type">Type of object to find.</param>
        /// <param name="additionals">Enumerable of objects to find from.</param>
        /// <param name="result">Found object. Null if not found.</param>
        /// <returns>True if object was found successfully; otherwise false.</returns>
        protected static bool TryFindAdditional(Type type, IEnumerable<object> additionals, out object result)
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

        /// <summary>Attempts to find a service of type from service provider.</summary>
        /// <param name="type">Type of service to find.</param>
        /// <param name="services">Service provider to get service from.</param>
        /// <param name="result">Found service. Null if not found.</param>
        /// <returns>True if service was found successfully; otherwise false.</returns>
        public static bool TryGetService(Type type, IServiceProvider services, out object result)
        {
            if (services == null)
            {
                result = null;
                return false;
            }
            result = services.GetService(type);
            return result != null;
        }

        /// <summary>Attempts to convert argument to parameter type.</summary>
        /// <param name="parameter">Parameter to convert argument to.</param>
        /// <param name="argIndex">Index of argument.</param>
        /// <param name="values">Builder values. Must contain Args and ArgumentConverterProvider.</param>
        /// <param name="result">Result of the conversion. Null if conversion failed.</param>
        /// <param name="error">Exception that occured when converting. Null if there was no exception.</param>
        /// <returns>True if converting was successful; otherwise false.</returns>
        protected static bool TryConvertArgument(ParameterInfo parameter, int argIndex, ParameterBuilderValues values, out object result, out Exception error)
        {
            if (argIndex < 0)
                throw new ArgumentException("Argument index cannot be negative", nameof(argIndex));
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            error = null;
            result = null;

            // check if we didn't run out of args
            if (values.Args == null || values.Args.Length - 1 < argIndex)
                return false;

            // get from provider
            IArgumentConverterProvider provider = values.ArgumentConverterProvider;
            IArgumentConverter converter = provider?.GetConverter(parameter);
            if (converter == null)
                return false;

            try
            {
                result = converter.Convert(parameter, values.Args[argIndex]);
                return true;
            }
            catch (Exception ex)
            {
                error = ex;
                return false;
            }
        }

        /// <summary>Attempts to create a generic ILogger&gt;T&lt;.</summary>
        /// <param name="type">Parameter type to create the logger for.</param>
        /// <param name="services">Service provider to get logger factory from.</param>
        /// <param name="result">Created logger. Null if failed to create.</param>
        /// <returns>True if successfully created the logger. Otherwise false.</returns>
        public static bool TryGetGenericLogger(Type type, IServiceProvider services, out object result)
        {
            result = null;
            if (!type.IsGenericType || !type.IsInterface)
                return false;
            if (!typeof(ILogger).IsAssignableFrom(type))
                return false;

            Type[] generics = type.GetGenericArguments();
            if (generics.Length != 1)
                return false;
            if (!(services.GetService(typeof(ILoggerFactory)) is ILoggerFactory factory))
                return false; 

            // we need to invoke the method using reflection to get generic ILogger
            MethodInfo creationMethod = typeof(LoggerFactoryExtensions)
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .First(x => x.ContainsGenericParameters && x.Name == "CreateLogger")
                .MakeGenericMethod(generics[0]);
            result = creationMethod.Invoke(null, new object[] { factory });
            return true;
        }
    }
}
