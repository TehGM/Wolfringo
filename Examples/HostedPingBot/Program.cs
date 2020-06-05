using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using TehGM.Wolfringo.Hosting;

namespace TehGM.Wolfringo.Examples.HostedPingBot
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // this is an example showing configuration for .NET Core 3.0+
            // for host configuration instructions in .NET Core 2.1 and 2.2, 
            // see https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-2.1
            IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("appsecrets-example.json", optional: true);
                    config.AddJsonFile("appsecrets.json", optional: true);
                    config.AddJsonFile($"appsecrets.{context.HostingEnvironment.EnvironmentName}.json", optional: true);
                })
                .ConfigureServices((context, services) =>
                {
                    services.Configure<HostedWolfClientOptions>(context.Configuration.GetSection("WolfClient"));
                    services.AddHostedService<HostedMessageHandler>();
                    services.AddWolfClient();
                })
                .ConfigureLogging((context, config) =>
                {
                    config.SetMinimumLevel(LogLevel.Debug);
                })
                .Build();
            await host.RunAsync();
        }
    }
}
