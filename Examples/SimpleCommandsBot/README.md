# Wolfringo example
This example projects shows basic usage of Wolfringo's Commands System.

This example shows how to create a bot client, connect it and use Commands System.  
This project shows how to use commands in [ExampleTransientCommandsHandler](ExampleTransientCommandsHandler.cs) class.  
To check how to make handler persistent (and use events etc in it), or how to use Constructor Injection or disposing, check [ExamplePersistentCommandsHandler](ExamplePersistentCommandsHandler.cs).
For examples on listening to WolfClient events, check [SimplePingBot example](..\SimplePingBot).

If you're running bot using [.NET Generic Host](https://docs.microsoft.com/en-gb/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-3.0) (such as ASP.NET Core application - but not only), check [HostedPingBot example](..\HostedPingBot) and [HostedCommandsBot example](..\HostedCommandsBot).

## Running locally
To be able to login, you need to create appsecrets.json file, and populate it with your bot's login credentials - see [appsecrets-example.json](appsecrets-example.json) for an example.