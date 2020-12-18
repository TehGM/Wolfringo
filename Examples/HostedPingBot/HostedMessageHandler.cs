using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using TehGM.Wolfringo.Messages;

namespace TehGM.Wolfringo.Examples.HostedPingBot
{
    /// <summary>Example message handler.</summary>
    /// <remarks><para>In .NET Core Host scenario, classes that use <see cref="IHostedWolfClient"/> should be registered as services. This allows other services, like <see cref="IHostedWolfClient"/> or
    /// others to be injected via constructor. This class shows a simple example how this can be achieved.</para>
    /// <para>This class implements <see cref="IHostedService"/>, even though it's start and stop methods do nothing. This allows for registration of the service using AddHostedService (look at code of <see cref="Program"/> for an example.
    /// Services added as hosted service will be created when the host starts - otherwise this service wouldn't be created at all, and thus not work.
    /// For more info on hosted services, see <see href="https://docs.microsoft.com/en-us/dotnet/architecture/microservices/multi-container-microservice-net-applications/background-tasks-with-ihostedservice">Microsoft Docs</see>.</para></remarks>
    public class HostedMessageHandler : IHostedService
    {
        private readonly IWolfClient _client;

        // can also be IWolfClient
        public HostedMessageHandler(IWolfClient client)
        {
            this._client = client;
            this._client.AddMessageListener<ChatMessage>(OnChatMessage);
        }

        private async void OnChatMessage(ChatMessage message)
        {
            if (message.IsPrivateMessage && message.IsText && message.Text.StartsWith("hello", StringComparison.OrdinalIgnoreCase))
            {
                await _client.ReplyTextAsync(message, "Hello there! What should I call you?").ConfigureAwait(false);

                // example of using Wolfringo.Utilities.Interactive package
                ChatMessage userResponse = await _client.AwaitNextPrivateByUserAsync(message.SenderID.Value, TimeSpan.FromMinutes(2));
                if (userResponse == null || !userResponse.IsText)
                    await _client.ReplyTextAsync(message, "Okay, goodbye... :(");
                else
                    await _client.ReplyTextAsync(userResponse, $"{userResponse.Text}? That's a great name!");
            }
        }

        // Implementing IHostedService ensures this class is created on start
        Task IHostedService.StartAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;
        Task IHostedService.StopAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}
