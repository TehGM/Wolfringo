using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using System;
using System.Threading.Tasks;
using TehGM.Wolfringo.Messages;
using TehGM.Wolfringo.Utilities;
using TehGM.Wolfringo.Commands;

namespace TehGM.Wolfringo.Examples.SimpleCommandsBot
{
    class Program
    {
        static IWolfClient _client;
        static async Task Main(string[] args)
        {
            // register to unhandled exceptions handling
            // this allows logging exceptions that were not handled with a try-catch block
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            // optional: create a logger
            ILogger<WolfClient> log = CreateLogger<WolfClient>();

            // create client and listen to events we're interested in
            _client = new WolfClient(log);
            _client.AddMessageListener<WelcomeEvent>(OnWelcome);        // these 2 callbacks are invoked if received message is a WolfEvent (first callback)

            // initialize commands system
            CommandsOptions options = new CommandsOptions()
            {
                Prefix = "!",                               // set prefix
                RequirePrefix = PrefixRequirement.Always,   // make prefix always required - can also for example require it in group only
                CaseSensitivity = false                     // make commands case insensitive
            };
            CommandsService commands = new CommandsService(_client, options, log: log);
            await commands.StartAsync();                    // calling StartAsync causes reload of all commands

            // start connection and prevent the application from closing
            await _client.ConnectAsync();
            await Task.Delay(-1);
        }

        // using Microsoft.Extensions.Logging.Console here for the sake of an example
        // in real life scenario, probably a full-fledged logging framework would be used, like Serilog, NLog or Log4Net
        private static ILogger<T> CreateLogger<T>()
        {
            ILoggerFactory loggerFactory = new LoggerFactory(
                            new[] { new ConsoleLoggerProvider((_, level) => true, true) }
                        );
            return loggerFactory.CreateLogger<T>();
        }

        /// <summary>Log exceptions that were unhandled.</summary>
        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ILogger log = CreateLogger<Program>();
            log.LogCritical(e.ExceptionObject as Exception, "An exception was unhandled");
        }

        /// <summary>Log exceptions that were raised by reconnector.</summary>
        private static void OnFailedToReconnect(object sender, UnhandledExceptionEventArgs e)
        {
            ILogger log = CreateLogger<WolfClientReconnector>();
            log.LogCritical(e.ExceptionObject as Exception, "Failed to reconnect");
        }

        private static async void OnWelcome(WelcomeEvent message)
        {
            // if reusing the token, user might be already logged in, so check that before requesting login
            if (message.LoggedInUser == null)
            {
                Config config = Config.Load();
                await _client.LoginAsync(config.Username, config.Password, WolfLoginType.Email);
            }
            await _client.SubscribeAllMessagesAsync();      // without this, bot will not receive any messages
        }
    }
}
