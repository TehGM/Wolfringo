using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using System;
using System.Threading.Tasks;
using TehGM.Wolfringo.Messages;
using System.Collections.Generic;

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
            _client.AddMessageListener<WelcomeMessage>(OnWelcome);
            _client.AddMessageListener<ChatMessage>(OnChatMessage);
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
            if (e.Message is ChatMessage msg)
            {
                if (msg.IsPrivateMessage)
                    await _client.SendAsync(ChatMessage.TextMessage(msg.SenderID.Value, msg.IsGroupMessage, "Hello there!"));
            }
        }

        private static async void OnWelcome(WelcomeMessage message)
        {
            Config config = Config.Load();
            await _client.LoginAsync(config.Username, config.Password);
            IEnumerable<WolfNotification> notifications = await _client.GetNotificationsAsync();
            // user should not be cached locally - always call GetUserAsync/GetCurrentUserAsync on client!
            WolfUser botUser = await _client.GetCurrentUserAsync();
            IEnumerable<WolfUser> contacts = await _client.GetContactListAsync();
            await _client.SetOnlineStateAsync(WolfOnlineState.Online);
            WolfGroup group = await _client.GetGroupAsync(1915722);
        }

        private static async void OnChatMessage(ChatMessage message)
        {
            if (message.IsPrivateMessage)
                await _client.SendAsync(ChatMessage.TextMessage(message.SenderID.Value, message.IsGroupMessage, "Hello there (using dispatcher)!!!"));
            _client.RemoveMessageListener<ChatMessage>(OnChatMessage);
        }
    }
}
