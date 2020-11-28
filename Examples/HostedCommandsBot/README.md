# Wolfringo.Hosting example
This example projects shows basic usage of Wolfringo's Commands System together with Wolfringo.Hosting and [.NET Generic Host](https://docs.microsoft.com/en-gb/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-3.0) (such as ASP.NET Core application - but not only).

This example shows how to create a bot client, connect it and use Commands System.  
This project doesn't have many commands examples - it shows simply how to use Commands System with [.NET Generic Host](https://docs.microsoft.com/en-gb/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-3.0). To see more examples regarding Commands System, check [SimpleCommandsBot example](../SimpleCommandsBot), and its [ExampleTransientCommandsHandler](../SimpleCommandsBot/ExampleTransientCommandsHandler.cs) and [ExamplePersistentCommandsHandler](../SimpleCommandsBot/ExamplePersistentCommandsHandler.cs).

If you're running bot without using [.NET Generic Host](https://docs.microsoft.com/en-gb/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-3.0), check [SimplePingBot example](../SimplePingBot) and [SimpleCommandsBot example](../SimpleCommandsBot).

## Running locally
To be able to login, you need to create appsecrets.json file, and populate it with your bot's login credentials - see [appsecrets-example.json](appsecrets-example.json) for an example.