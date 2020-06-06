using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TehGM.Wolfringo.Hosting;

namespace TehGM.Wolfringo.Examples.HostedPingBot
{
    class Program
    {
        static void Main(string[] args)
        {
            // this is an example showing configuration for .NET Core 3.0+
            // for host configuration instructions in .NET Core 2.1 and 2.2, 
            // see https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-2.1
            IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    // add example json - just as a presentation
                    config.AddJsonFile("appsecrets-example.json", optional: true);
                    // add actual app secrets if exist - secrets should not be included in git repositories!
                    config.AddJsonFile("appsecrets.json", optional: true);
                    config.AddJsonFile($"appsecrets.{context.HostingEnvironment.EnvironmentName}.json", optional: true);
                })
                .ConfigureServices((context, services) =>
                {
                    // configure and add hosted wolf client
                    services.Configure<HostedWolfClientOptions>(context.Configuration.GetSection("WolfClient"));
                    services.AddWolfClient();
                    // classes that are not required by other services, such as this message handler, should be registered as hosted
                    services.AddHostedService<HostedMessageHandler>();
                })
                .ConfigureLogging((context, config) =>
                {
                    // configure logging here
                    // see your logging library configuration instructions
                    config.SetMinimumLevel(LogLevel.Debug);
                })
                .Build();
            host.RunAsync().GetAwaiter().GetResult();
        }
    }
}
