using Microsoft.Extensions.Logging;
using System;

namespace TehGM.Wolfringo.Utilities
{
    /// <summary>Utility class for logging exceptions.</summary>
    /// <remarks><para>By default, if exception is caught, and exception is logged inside of the handling block, the log scope will be lost. 
    /// The way around this is to log the exception using `when` keyword.<br/>
    /// This class contains methods that return true so they can be used with `when` syntax.</para>
    /// <para>Usage sample:<code>
    /// try
    /// {
    ///     // ...
    /// }
    /// catch (Exception ex) when (ex.LogAsError(_log, "Exception has occured")) { }
    /// </code></para>
    /// <para>More information: https://andrewlock.net/how-to-include-scopes-when-logging-exceptions-in-asp-net-core/#exceptions-inside-scope-blocks-lose-the-scope </para></remarks>
    public static class ExceptionLoggingHelper
    {
        /// <summary>Logs exception with critical log level.</summary>
        /// <remarks><para>See <see cref="ExceptionLoggingHelper"/> for more information.</para>
        /// <para>This method is null-logger-safe - if <paramref name="log"/> is null, message and exception won't be logged.</para></remarks>
        /// <param name="exception">Exception message to log.</param>
        /// <param name="log">Logger instance.</param>
        /// <param name="message">Log message template.</param>
        /// <param name="args">Structured log message arguments.</param>
        /// <returns>Always returns true.</returns>
        public static bool LogAsCritical(this Exception exception, ILogger log, string message, params object[] args)
        {
            log?.LogCritical(exception, message, args);
            return true;
        }
        /// <summary>Logs exception with error log level.</summary>
        /// <remarks><para>See <see cref="ExceptionLoggingHelper"/> for more information.</para>
        /// <para>This method is null-logger-safe - if <paramref name="log"/> is null, message and exception won't be logged.</para></remarks>
        /// <param name="exception">Exception message to log.</param>
        /// <param name="log">Logger instance.</param>
        /// <param name="message">Log message template.</param>
        /// <param name="args">Structured log message arguments.</param>
        /// <returns>Always returns true.</returns>
        public static bool LogAsError(this Exception exception, ILogger log, string message, params object[] args)
        {
            log?.LogError(exception, message, args);
            return true;
        }
        /// <summary>Logs exception with warning log level.</summary>
        /// <remarks><para>See <see cref="ExceptionLoggingHelper"/> for more information.</para>
        /// <para>This method is null-logger-safe - if <paramref name="log"/> is null, message and exception won't be logged.</para></remarks>
        /// <param name="exception">Exception message to log.</param>
        /// <param name="log">Logger instance.</param>
        /// <param name="message">Log message template.</param>
        /// <param name="args">Structured log message arguments.</param>
        /// <returns>Always returns true.</returns>
        public static bool LogAsWarning(this Exception exception, ILogger log, string message, params object[] args)
        {
            log?.LogWarning(exception, message, args);
            return true;
        }
        /// <summary>Logs exception with information log level.</summary>
        /// <remarks><para>See <see cref="ExceptionLoggingHelper"/> for more information.</para>
        /// <para>This method is null-logger-safe - if <paramref name="log"/> is null, message and exception won't be logged.</para></remarks>
        /// <param name="exception">Exception message to log.</param>
        /// <param name="log">Logger instance.</param>
        /// <param name="message">Log message template.</param>
        /// <param name="args">Structured log message arguments.</param>
        /// <returns>Always returns true.</returns>
        public static bool LogAsInformation(this Exception exception, ILogger log, string message, params object[] args)
        {
            log?.LogInformation(exception, message, args);
            return true;
        }
        /// <summary>Logs exception with debug log level.</summary>
        /// <remarks><para>See <see cref="ExceptionLoggingHelper"/> for more information.</para>
        /// <para>This method is null-logger-safe - if <paramref name="log"/> is null, message and exception won't be logged.</para></remarks>
        /// <param name="exception">Exception message to log.</param>
        /// <param name="log">Logger instance.</param>
        /// <param name="message">Log message template.</param>
        /// <param name="args">Structured log message arguments.</param>
        /// <returns>Always returns true.</returns>
        public static bool LogAsDebug(this Exception exception, ILogger log, string message, params object[] args)
        {
            log?.LogDebug(exception, message, args);
            return true;
        }
        /// <summary>Logs exception with trace log level.</summary>
        /// <remarks><para>See <see cref="ExceptionLoggingHelper"/> for more information.</para>
        /// <para>This method is null-logger-safe - if <paramref name="log"/> is null, message and exception won't be logged.</para></remarks>
        /// <param name="exception">Exception message to log.</param>
        /// <param name="log">Logger instance.</param>
        /// <param name="message">Log message template.</param>
        /// <param name="args">Structured log message arguments.</param>
        /// <returns>Always returns true.</returns>
        public static bool LogAsTrace(this Exception exception, ILogger log, string message, params object[] args)
        {
            log?.LogTrace(exception, message, args);
            return true;
        }
    }
}
