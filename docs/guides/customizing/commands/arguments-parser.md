---
uid: Guides.Customizing.Commands.ArgumentsParser
title: Customizing Wolfringo - Arguments Parser
---

# Customizing Arguments Parser
@TehGM.Wolfringo.Commands.Parsing.IArgumentsParser is used by Wolfringo Commands System whenever Standard commands are involved (Regex commands do not use the Arguments Parser). It is what separates arguments by space, and groups them together if put in quotes like `"foo bar"` etc.

Like many other parts of Wolfringo Commands System, @TehGM.Wolfringo.Commands.Parsing.IArgumentsParser is resolved by @TehGM.Wolfringo.Commands.CommandsService from @System.IServiceProvider - and therefore you can fully overwrite the way Worlfringo Standard commands parse arguments. Default @TehGM.Wolfringo.Commands.Parsing.ArgumentsParser implementation also allows changing its options, so you can tune it to your needs even without writing your custom one!

## Changing Options of Default Parser
Default @TehGM.Wolfringo.Commands.Parsing.ArgumentsParser implementation takes @TehGM.Wolfringo.Commands.Parsing.ArgumentsParserOptions as its constructor argument. You can use that to change the behaviour of the default Arguments Parser easily before Commands Service starts.

Options you can change include:
- @TehGM.Wolfringo.Commands.Parsing.ArgumentsParserOptions.BaseMarker - the character that is used to split arguments in most scenarios. Defaults to space (so arguments are per-word).
- @TehGM.Wolfringo.Commands.Parsing.ArgumentsParserOptions.BlockMarkers - dictionary of opening and closing characters that define a block. Defaults to 2 spaces, `[` and `]`, `(` and `)`, and `"` and `"`.
- @TehGM.Wolfringo.Commands.Parsing.ArgumentsParserOptions.InitialBlockSizeAllocation - performance option. Changes the default memory allocation per argument parsed before it grows. Defaults to 8, which should be enough for most commands arguments.

### [Without Wolfringo.Hosting (Normal Bot)](#tab/configuring-normal-bot)
Note: instructions skip logger setup. See [Logging guide](xref:Guides.Features.Logging) for details.
1. Manually create an instance of @TehGM.Wolfringo.Commands.Parsing.ArgumentsParserOptions.
2. Customize the options however you need.
3. Create a new instance of @TehGM.Wolfringo.Commands.Parsing.ArgumentsParser, passing your options instance via constructor.
4. Register your @TehGM.Wolfringo.Commands.Parsing.ArgumentsParser instance with @Microsoft.Extensions.DependencyInjection.IServiceCollection.
5. Build your service provider and pass it to @TehGM.Wolfringo.Commands.CommandsService constructor.
```csharp
ArgumentsParserOptions argsParserOptions = new ArgumentsParserOptions();
argsParserOptions.BaseMarker = '-';
ArgumentsParser customArgsParser = new ArgumentsParser(argsParserOptions);
IServiceCollection services = new ServiceCollection()
    .AddSingleton<IArgumentsParser(customArgsParser);
// add any other services as needed

_client = new WolfClient(logger);                                           // create wolf client
CommandsService commands = new CommandsService(_client, options, logger,    // initialize commands service
    services.BuildServiceProvider());                           // add Dependency Injection Service provider
```

### [With Wolfringo.Hosting (.NET Generic Host/ASP.NET Core)](#tab/configuring-hosted-bot)
1. Configure @TehGM.Wolfringo.Commands.Parsing.ArgumentsParserOptions.
2. Customize the options however you need.
```csharp
services.Configure<ArgumentsParserOptions>(options =>
{
    options.BaseMarker = '-';
});
```

> [!TIP]
> You can also configure these options using other means of .NET Generic Host configuration, such as JSON file.

***

## Custom Parser
While the default @TehGM.Wolfringo.Commands.Parsing.ArgumentsParser implementation allows for some customizability, you can get full control by writing your own implementation. To do so, simply create a class implementing @TehGM.Wolfringo.Commands.Parsing.IArgumentsParser interface. Once your class is complete, you can register it with @System.IServiceProvider as explained in [Introduction](xref:Guides.Customizing.Intro).

You can view default implementation [on GitHub](https://github.com/TehGM/Wolfringo/blob/master/Wolfringo.Commands/Parsing/ArgumentsParser.cs) for reference.