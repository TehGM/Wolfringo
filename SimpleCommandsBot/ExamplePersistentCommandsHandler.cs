using System;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using TehGM.Wolfringo.Commands;

namespace TehGM.Wolfringo.Examples.SimpleCommandsBot
{
    /*** Example: persistent command handler
     * Persistent command handler is kept in memory for as long as CommandService is alive - they won't be re-created for each new command execution.
     * Instead, they'll be created once when CommandService starts. This is useful when you need to keep data in memory, or subscribe to any events.
     * 
     * Note: if your command handler implements IDisposable, its Dispose() method will be called once when CommandService is being disposed.
     ***/
    [CommandsHandler(IsPersistent = true)]
    class ExamplePersistentCommandsHandler : IDisposable
    {
        private readonly IWolfClient _client;
        private readonly ILogger _log;

        /*** Example: Constructor
         * This example shows a command handler constructor.
         * Default CommandHandlerProvider will automatically inject any service that was provided to IServiceProvider.
         * Services that have default values will be treated as optional - if they exist, they'll be injected, otherwise default value will be used.
         * 
         * Constructors are attempted in order of the amount of parameters they take. 
         * Highest amount of parameters will be attempted first, as this gives opportunity to resolve as many parameters as possible.
         * If non-optionals parameters can't be resolved for any of the constructors, an exception will be thrown, indicating an error.
         * 
         * If [CommandHandlerConstructor] attribute is present on any of the constructors, all constructors without this attribute will be ignored.
         * This attribute is optional - if none of the constructors have it, all constructors will be attempted.
         * 
         * Note: constructors work the same with non-persistent handlers - but they're more useful with persistent ones, as non-persistent ones have lifetime of a command execution.
         ***/
        [CommandsHandlerConstructor]
        public ExamplePersistentCommandsHandler(IWolfClient client, ILogger log = null)
        {
            this._client = client;
            this._client.Disconnected += OnClientDisconnected;
            this._log = log;
        }
        // this constructor won't be used, because the other constructor has [CommandHandlerConstructor] attribute
        public ExamplePersistentCommandsHandler(IWolfClient client, string specialValue) : this(client) { }

        private void OnClientDisconnected(object sender, EventArgs e)
        {
            // log could be null, as it's optional in constructor
            if (_log == null)
                return;
            _log.LogDebug("Oh no, client disconnected! :(");
        }

        public void Dispose()
        {
            this._client.Disconnected -= OnClientDisconnected;
        }


        /*** Example: command methods.
         * Command methods themselves work exactly the same as with non-persistent handlers - look there for more command examples.
         ***/
        [RegexCommand("^log (.+)$")]
        public void CmdLog(CommandContext context, Match match)
        {
            if (_log == null)
                return;

            using (_log.BeginCommandScope(context, this))
                _log.LogInformation(match.Groups[1].Value);
        }
    }
}
