---
uid: Guides.Introduction
---

# Introduction
Welcome to Wolfringo library usage guide. If you don't know what Wolfringo is - please check [Home Page](/).

Please check navigation on the left ⬅ (or in menu ☰ on mobile devices) for more articles.

### Requirements
Wolfringo library should work with any framework version supported by [.NET Standard 2.0](https://docs.microsoft.com/en-gb/dotnet/standard/net-standard) or greater. However, it is guaranteed only with .NET Core 2.0 and up and .NET 5 and up - I did not test .NET Framework, Mono, Xamarin or Unity.

This documentation assumes you have decent knowledge of [C# Language](https://docs.microsoft.com/en-gb/dotnet/csharp/programming-guide/) and [asynchronous programing](https://docs.microsoft.com/en-gb/dotnet/csharp/programming-guide/concepts/async/).

##### If using Wolfringo.Hosting
[Wolfringo.Hosting](https://www.nuget.org/packages/Wolfringo.Hosting) is an ***optional*** addon package that enables support for [.NET Generic Host](https://docs.microsoft.com/en-gb/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-3.0)/[ASP.NET Core](https://docs.microsoft.com/en-gb/aspnet/core/fundamentals/host/web-host?view=aspnetcore-3.0). If you're willing to use it, you should have basic understanding of these .NET scenarios.

Wolfringo.Hosting fully supports breaking changes introduced with [ASP.NET Core 3.0](https://docs.microsoft.com/en-gb/aspnet/core/release-notes/aspnetcore-3.0?view=aspnetcore-3.0) through multi-targeting. Wolfringo.Hosting only works with .NET Core.

## Getting Started
Installation and creation of your first bot is explained in [Getting Started](xref:Guides.GettingStarted.Installation) section.

For more details on how to use Wolfringo, check [Working with Wolfringo](xref:Guides.Features.Logging) section.

To learn how to use Commands System for your bots' commands, check [Commands System](xref:Guides.Commands.Intro) section.

## Customizing Wolfringo
Wolfringo utilizes Dependency Injection to allow customization of the library behaviour - many parts of both client and commands system can be configured or even replaced without the need to rewrite everything.

Check [Customizing guides](xref:Guides.Customizing.Intro) for more info!

## Questions?
To ask questions or share ideas, feel free to start a new [GitHub Discussion](https://github.com/TehGM/Wolfringo/discussions).

Any bugs? Requesting a specific feature? Please create a new [Issue on GitHub](https://github.com/TehGM/Wolfringo/issues).

To contact me directly, you can PM me on [Discord](https://discord.com/users/247081094799687682). You can also find me on [WOLF](https://wolf.live/u/2644384) - but I cannot guarantee quick replies there, so Discord is preferred.