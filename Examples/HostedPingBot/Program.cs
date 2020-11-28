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
                    // add actual app secrets if exist - secrets should not be included in git repositories!
                    config.AddJsonFile("appsecrets.json", optional: true);
                    config.AddJsonFile($"appsecrets.{context.HostingEnvironment.EnvironmentName}.json", optional: true);
                })
                .ConfigureServices((context, services) =>
                {
                    // configure and add hosted wolf client
                    services.Configure<HostedWolfClientOptions>(context.Configuration.GetSection("WolfClient"));
                    services.AddWolfClient()
                        /** Commented methods below override configuration from appsettings.json and appsecrets.json - use them if you want to override, or do not use config files **/
                        //.SetCredentials("login", "password")              -- sets bot credentials. Note: it's recommended to not use this method, 
                        //                                                      and stick to config file that is excluded from the repository - such as appsettings.json in this example
                        //.SetAutoReconnectAttempts(15)                     -- sets auto reconnect attempts limit
                        //.SetAutoReconnectDelay(TimeSpan.FromSeconds(1))   -- sets delay between auto reconnect attempts
                        //.SetInfiniteAutoReconnectAttempts()               -- removes auto reconnect attempts limit, retrying until successful. Has same effect as setting attempts to -1.
                        //.DisableAutoReconnect()                           -- disables auto reconnect completely. Has same effect as setting attempts to 0.
                        //.SetDefaultServerURL()                            -- uses default WOLF server URL
                        //.SetBetaServerURL()                               -- uses Release Candidate WOLF server URL
                        //.SetServerURL("wss://v3.palringo.com:3051")       -- allows setting custom WOLF server URL to connect to
                        ;


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
