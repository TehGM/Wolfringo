using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace TehGM.Wolfringo.Commands
{
    /// <summary>Extensions for logging commands.</summary>
    public static class CommandLoggingExtensions
    {
        /// <summary>Creates a log scope for command context.</summary>
        /// <param name="log">Logger.</param>
        /// <param name="context">Command context to create scope for.</param>
        /// <param name="handler">Handler to use in scope arguments.</param>
        /// <param name="methodName">Name of the method creating the scope.</param>
        /// <returns>Log scope.</returns>
        public static IDisposable BeginCommandScope(this ILogger log, ICommandContext context, object handler = null, [CallerMemberName] string methodName = null)
            => BeginCommandScope(log, context, handler?.GetType(), methodName);

        /// <summary>Creates a log scope for command context.</summary>
        /// <param name="log">Logger.</param>
        /// <param name="context">Command context to create scope for.</param>
        /// <param name="handlerType">Handler to use in scope arguments.</param>
        /// <param name="methodName">Name of the method creating the scope.</param>
        /// <returns>Log scope.</returns>
        public static IDisposable BeginCommandScope(this ILogger log, ICommandContext context, Type handlerType = null, [CallerMemberName] string methodName = null)
        {
            if (log == null)
                return null;
            Dictionary<string, object> state = new Dictionary<string, object>
            {
                { "Command.SenderID", context.Message.SenderID.Value },
                { "Command.MessageID", context.Message.Timestamp.ToString() },
                { "Command.RecipientID", context.Message.RecipientID },
                { "Command.RecipientType", context.Message.IsGroupMessage ? "Group" : "Private" }
            };
            if (!string.IsNullOrWhiteSpace(methodName))
                state.Add("Command.Method", methodName);
            if (handlerType != null)
                state.Add("Command.Handler", handlerType.Name);
            return log.BeginScope(state);
        }
    }
}
