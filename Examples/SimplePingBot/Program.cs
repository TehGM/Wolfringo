using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using System;
using System.Threading.Tasks;
using TehGM.Wolfringo.Messages;

namespace TehGM.Wolfringo.Examples.SimplePingBot
{
    class Program
    {
        static IWolfClient _client;
        static async Task Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            ILogger<WolfClient> log = CreateLogger<WolfClient>();

            _client = new WolfClient(log);
            _client.MessageReceived += OnMessageReceived;
            await _client.ConnectAsync();
            await Task.Delay(-1);
        }

        // using Microsoft.Extensions.Logging.Console here for the sake of an example
        // in real life scenario, probably a full-fledged logging framework would be used, like Serilog, NLog or Log4Net
        private static ILogger<T> CreateLogger<T>()
        {
            ILoggerFactory loggerFactory = new LoggerFactory(
                            new[] { new ConsoleLoggerProvider((_, level) => level != LogLevel.Trace, true) }
                        );
            return loggerFactory.CreateLogger<T>();
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                ILogger log = CreateLogger<Program>();
                log.LogCritical(e.ExceptionObject as Exception, "An exception was unhandled");
            }
            catch
            {
                Console.WriteLine("ERROR: " + e.ExceptionObject.ToString());
            }
        }

        private static async void OnMessageReceived(object sender, WolfMessageEventArgs e)
        {
            //Console.WriteLine("Message received: " + obj.Command);
            if (e.Message is WelcomeMessage)
            {
                Config config = Config.Load();
                await _client.LoginAsync(config.Username, config.Password);
            }
            else if (e.Message is ChatMessage msg)
            {
                //if (msg.IsText)
                //    Console.WriteLine(msg.Text);
                if (msg.IsPrivateMessage)
                    await _client.SendAsync(ChatMessage.TextMessage(msg.SenderID.Value, msg.IsGroupMessage, "Hello there!"));
            }
        }
    }
}
