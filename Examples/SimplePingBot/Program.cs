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
            // register to unhandled exceptions handling
            // this allows logging exceptions that were not handled with a try-catch block
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            // optional: create a logger
            ILogger<WolfClient> log = CreateLogger<WolfClient>();

            // create client and listen to events we're interested in
            _client = new WolfClient(log);
            _client.MessageReceived += OnMessageReceived;               // This event is raised when client receives and parses any message type.
            _client.AddMessageListener<WelcomeEvent>(OnWelcome);        // these 2 callbacks are invoked if received message is a WolfEvent (first callback)
            _client.AddMessageListener<ChatMessage>(OnChatMessage);     // or a chat message (second callback).
            _client.ErrorRaised += OnError;                             // this event is raised when there was an error during background processing

            // reconnector is part of Wolfringo.Utilities package
            WolfClientReconnector reconnector = new WolfClientReconnector(_client);
            reconnector.FailedToReconnect += OnFailedToReconnect;

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

        /// <summary>This event is raised when client receives and parses any message type.</summary>
        private static async void OnMessageReceived(object sender, WolfMessageEventArgs e)
        {
            if (e.Message is ChatMessage msg)
            {
                // reply only to private text messages that start with "hello"
                if (msg.IsPrivateMessage && msg.IsText && msg.Text.StartsWith("hello", StringComparison.OrdinalIgnoreCase))
                    await _client.ReplyTextAsync(msg, "Hello there!");
            }
        }

        private static async void OnWelcome(WelcomeEvent message)
        {
            // if reusing the token, user might be already logged in, so check that before requesting login
            if (message.LoggedInUser == null)
            {
                Config config = Config.Load();
                await _client.LoginAsync(config.Username, config.Password);
            }
            // user should not be cached locally - always call GetUserAsync/GetCurrentUserAsync on client!
            WolfUser user = await _client.GetCurrentUserAsync();
            // same applies to groups - always call GetGroupsAsync!
            IEnumerable<WolfGroup> groups = await _client.GetCurrentUserGroupsAsync();
        }

        private static async void OnChatMessage(ChatMessage message)
        {
            // reply only to private text messages that start with "hello"
            if (message.IsPrivateMessage && message.IsText && message.Text.StartsWith("hello", StringComparison.OrdinalIgnoreCase))
            {
                await _client.ReplyTextAsync(message, "Hello there (using dispatcher)!!!");
                // an example showing how listener can be removed
                _client.RemoveMessageListener<ChatMessage>(OnChatMessage);
            }
        }

        private static async void OnError(object sender, UnhandledExceptionEventArgs e)
        {
            // handle non-sending errors here
            // example: error when subscribing to messages on reconnection (caused by a bug in WOLF server)
            if (e.ExceptionObject is MessageSendingException msgSendingException)
            {
                if (msgSendingException.SentMessage is SubscribeToPmMessage || msgSendingException.SentMessage is SubscribeToGroupMessage)
                {
                    await _client.LogoutAsync();
                    await _client.DisconnectAsync();
                    // uncomment the line below if not using any auto-reconnecting mechanism, such as WolfClientReconnector
                    // if something is handling auto-reconnection, leave this line commented
                    //await _client.ConnectAsync();
                }
            }
        }
    }
}
