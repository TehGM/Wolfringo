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

            // create client with builder
            _client = new WolfClientBuilder()
                // add logging
                .WithLogging(CreateLoggerFactory())
                // add commands
                .WithCommands(commands =>
                {
                    // when using .WithCommands, commands.WithWolfClient will be overwritten, so we don't need to call it
                    // .WithCommands will also automatically use the same IServiceCollection as WolfClientBuilder
                    // for this reason, we can skip calling commands.WithLogging as well
                    commands
                        .WithPrefix("!")                                    // set prefix
                        .WithPrefixRequirement(PrefixRequirement.Always)    // make prefix always required - can also for example require it in group only
                        .WithCaseSensitivity(false)                         // make commands case insensitive
                        .WithDefaultHelpCommand();                          // enable the default, built-in help command
                })
                // add reconnector
                .WithAutoReconnection(reconnector =>
                {
                    reconnector.ReconnectAttempts = 100;
                })
                // finally, build the client
                .Build(); 
            
            // register client events
            _client.AddMessageListener<WelcomeEvent>(OnWelcome);        // this callback is invoked if received message is a WolfEvent

            // start connection and prevent the application from closing
            await _client.ConnectAsync();
            await Task.Delay(-1);
        }

        // using Microsoft.Extensions.Logging.Console here for the sake of an example
        // in real life scenario, probably a full-fledged logging framework would be used, like Serilog, NLog or Log4Net
        private static ILoggerFactory CreateLoggerFactory()
        {
            return new LoggerFactory(
                new[] { new ConsoleLoggerProvider((_, level) => true, true) }
            );
        }

        /// <summary>Log exceptions that were unhandled.</summary>
        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ILogger log = CreateLoggerFactory().CreateLogger<Program>();
            log.LogCritical(e.ExceptionObject as Exception, "An exception was unhandled");
        }

        /// <summary>Log exceptions that were raised by reconnector.</summary>
        private static void OnFailedToReconnect(object sender, UnhandledExceptionEventArgs e)
        {
            ILogger log = CreateLoggerFactory().CreateLogger<WolfClientReconnector>();
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
