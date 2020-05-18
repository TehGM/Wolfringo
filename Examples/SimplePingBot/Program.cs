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
            _client = new WolfClient();
            _client.MessageReceived += OnMessageReceived;
            await _client.ConnectAsync();
            await Task.Delay(-1);
        }

        private static async void OnMessageReceived(IWolfMessage obj)
        {
            Console.WriteLine("Message received: " + obj.Command);
            if (obj is WelcomeMessage)
            {
                Config config = Config.Load();
                await _client.SendAsync(new LoginMessage(config.Username, config.Password));
                await Task.Delay(250);
                await _client.SendAsync(new SubscribeToPmMessage());
            }
        }
    }
}
