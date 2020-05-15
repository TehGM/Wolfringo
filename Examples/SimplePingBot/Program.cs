using System;
using System.Threading.Tasks;

namespace TehGM.Wolfringo.Examples.SimplePingBot
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IWolfClient client = new WolfClient();
            client.Connected += OnClientConnected;
            client.Disconnected += OnClientDisconnected;
            client.MessageReceived += OnMessageReceived;
            await client.ConnectAsync();
            await Task.Delay(-1);
        }

        private static void OnMessageReceived(IWolfMessage obj)
        {
            Console.WriteLine("Message received: " + obj.Command);
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
