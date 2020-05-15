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
            _client.Connected += OnClientConnected;
            _client.Disconnected += OnClientDisconnected;
            _client.MessageReceived += OnMessageReceived;
            await _client.ConnectAsync();
            await Task.Delay(-1);
        }

        private static async void OnMessageReceived(IWolfMessage obj)
        {
            Console.WriteLine("Message received: " + obj.Command);
            if (obj is WelcomeMessage)
            {
                await _client.SendAsync(new LoginMessage("", ""));
            }
        }

        private static void OnClientConnected()
        {
            Console.WriteLine("Connected");
        }

        private static void OnClientDisconnected()
        {
            Console.WriteLine("Disconnected");
        }
    }
}
