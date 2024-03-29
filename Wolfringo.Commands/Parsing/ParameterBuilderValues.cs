﻿using System;
using System.Collections.Generic;
using System.Threading;
using TehGM.Wolfringo.Commands.Initialization;

namespace TehGM.Wolfringo.Commands.Parsing
{
    /// <summary>Values to use when building parameters.</summary>
    public class ParameterBuilderValues
    {
        /// <summary>Args parsed from the message.</summary>
        public string[] Args { get; set; }
        /// <summary>Command context.</summary>
        public ICommandContext Context { get; set; }
        /// <summary>Services provider for Dependency Injection.</summary>
        public IServiceProvider Services { get; set; }
        /// <summary>Provider of argument converters.</summary>
        public IArgumentConverterProvider ArgumentConverterProvider { get; set; }
        /// <summary>Cancellation token to inject into command.</summary>
        public CancellationToken CancellationToken { get; set; }
        /// <summary>Any additional objects that can be used when injecting dependencies.</summary>
        public IEnumerable<object> AdditionalObjects { get; set; }
        /// <summary>Instance of a command that is being executed.</summary>
        public ICommandInstance CommandInstance { get; set; }
        /// <summary>The message text, not including prefix and command trigger.</summary>
        public string ArgsText { get; set; }
        /// <summary>Options for command context, with command's overrides applied.</summary>
        public ICommandOptions Options { get; set; }
    }
}
