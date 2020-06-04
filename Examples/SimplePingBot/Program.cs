using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using System;
using System.Threading.Tasks;
using TehGM.Wolfringo.Messages;
using System.Collections.Generic;
using TehGM.Wolfringo.Utilities;

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
            _client.AddMessageListener<WelcomeEvent>(OnWelcome);
            _client.AddMessageListener<ChatMessage>(OnChatMessage);

            // reconnector is part of Wolfringo.Utilities package
            WolfClientReconnector reconnector = new WolfClientReconnector(_client);
            reconnector.FailedToReconnect += OnFailedToReconnect;

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
            ILogger log = CreateLogger<Program>();
            log.LogCritical(e.ExceptionObject as Exception, "An exception was unhandled");
        }

        private static void OnFailedToReconnect(object sender, UnhandledExceptionEventArgs e)
        {
            ILogger log = CreateLogger<WolfClientReconnector>();
            log.LogCritical(e.ExceptionObject as Exception, "Failed to reconnect");
        }

        private static async void OnMessageReceived(object sender, WolfMessageEventArgs e)
        {
            if (e.Message is ChatMessage msg)
            {
                if (msg.IsPrivateMessage)
                    await _client.SendAsync(ChatMessage.TextMessage(msg.SenderID.Value, msg.IsGroupMessage, "Hello there!"));
            }
        }

        private static async void OnWelcome(WelcomeEvent message)
        {
            Config config = Config.Load();
            await _client.LoginAsync(config.Username, config.Password);
            // user should not be cached locally - always call GetUserAsync/GetCurrentUserAsync on client!
            WolfUser user = await _client.GetCurrentUserAsync();
            // same applies to groups - always call GetGroupsAsync!
            IEnumerable<WolfGroup> groups = await _client.GetCurrentUserGroupsAsync();
        }

        private static async void OnChatMessage(ChatMessage message)
        {
            if (message.IsPrivateMessage)
            {
                await _client.SendAsync(ChatMessage.TextMessage(message.SenderID.Value, message.IsGroupMessage, "Hello there (using dispatcher)!!!"));
                // an example showing how listener can be removed
                _client.RemoveMessageListener<ChatMessage>(OnChatMessage);
            }
        }
    }
}
