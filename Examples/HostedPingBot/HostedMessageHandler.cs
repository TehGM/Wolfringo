using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using TehGM.Wolfringo.Hosting;
using TehGM.Wolfringo.Messages;

namespace TehGM.Wolfringo.Examples.HostedPingBot
{
    public class HostedMessageHandler : IHostedService
    {
        private readonly IHostedWolfClient _client;

        // can also be IWolfClient
        public HostedMessageHandler(IHostedWolfClient client)
        {
            this._client = client;
            this._client.AddMessageListener<ChatMessage>(OnChatMessage);
        }

        private async void OnChatMessage(ChatMessage message)
        {
            if (message.IsPrivateMessage)
                await _client.RespondWithTextAsync(message, "Hello there!").ConfigureAwait(false);
        }

        Task IHostedService.StartAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;

        Task IHostedService.StopAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}
