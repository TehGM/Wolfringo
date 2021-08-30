using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TehGM.Wolfringo.Commands;
using TehGM.Wolfringo.Hosting;

namespace TehGM.Wolfringo.Examples.HostedCommandsBot
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
                    // configure options for client and commands
                    services.Configure<HostedWolfClientOptions>(context.Configuration.GetSection("WolfClient"));
                    services.Configure<CommandsOptions>(context.Configuration.GetSection("Commands"));

                    // add wolf client
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

                    // add commands
                    services.AddWolfringoCommands()
                        /** Values from appsettings.json - uncomment if you're not using appsettings.json for configuration, or want to override! **/
                        //.SetPrefix("!")           
                        //.SetCaseSensitive(false)
                        //.SetPrefixRequirement(PrefixRequirement.Group)
                        //.EnableDefaultHelpCommand()                   -- enables default help command

                        /** by default, all handlers from the same assembly as the bot will be loaded - commented example below shows how to override. **/
                        //.RemoveDefaultHandlers()                      -- this will stop commands from loading current assembly by default 
                        //.AddHandler<HostedCommandsHandler>()          -- this will add a specific commands handler
                        //.AddHandlers(typeof(IWolfClient).Assembly)    -- this shows how to add entire assemblies - for example, when commands are a part of some nuget package
                        ;
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
