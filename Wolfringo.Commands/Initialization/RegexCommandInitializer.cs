using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using TehGM.Wolfringo.Commands.Instances;

namespace TehGM.Wolfringo.Commands.Initialization
{
    public class RegexCommandInitializer : ICommandInitializer
    {
        private readonly IServiceProvider _services;
        private readonly IWolfClient _client;

        private readonly IDictionary<Type, HandlerDescriptor> _knownHandlers;

        public RegexCommandInitializer(IWolfClient client, IServiceProvider services)
        {
            this._services = services;
            this._client = client;
        }

        public ICommandInstance InitializeCommand(CommandAttributeBase commandAttribute, MethodInfo method)
        {
            // validate this is a correct command attribute type
            if (!(commandAttribute is RegexCommandAttribute regexCommand))
                throw new ArgumentException($"{this.GetType().Name} can only be used with {typeof(RegexCommandAttribute).Name} commands", nameof(commandAttribute));

            Type handlerType = regexCommand.GetType().DeclaringType;
            if (!_knownHandlers.TryGetValue(handlerType, out HandlerDescriptor handlerDescriptor))
            {
                handlerDescriptor = commandAttribute.GetType().FindHandlerDescriptor(this._client, this._services);
                _knownHandlers.Add(handlerType, handlerDescriptor);
            }

            // prepare regex
            RegexOptions options = regexCommand.Options;
            if (regexCommand.OverrideCaseInsensitive == null || regexCommand.OverrideCaseInsensitive == true)
                options |= RegexOptions.IgnoreCase;
            Regex regex = new Regex(regexCommand.Pattern, options);

            // init handler and instance
            object handler = handlerDescriptor.CreateInstance();
            return new RegexCommandInstance(regex, method, handler);
        }
    }
}
