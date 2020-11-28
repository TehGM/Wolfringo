# Wolfringo.Hosting example
This example projects shows basic usage of Wolfringo.Hosting's HostedWolfClient with [.NET Generic Host](https://docs.microsoft.com/en-gb/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-3.0) (such as ASP.NET Core application - but not only).

This example shows how to create a bot client, connect it and listen to its events.  
Commands System is not used in this project. For examples on Commands System usage, check [HostedCommandsBot example](../HostedCommandsBot).

If you're running bot without using [.NET Generic Host](https://docs.microsoft.com/en-gb/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-3.0), check [SimplePingBot example](../SimplePingBot) and [SimpleCommandsBot example](../SimpleCommandsBot).

## Running locally
To be able to login, you need to create appsecrets.json file, and populate it with your bot's login credentials - see [appsecrets-example.json](appsecrets-example.json) for an example.