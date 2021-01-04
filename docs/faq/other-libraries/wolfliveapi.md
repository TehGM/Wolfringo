---
uid: FAQ.OtherLibs.WolfLiveApi
---

# FAQ - WolfLive.Api and Wolfringo
[WolfLive.Api](https://github.com/calico-crusade/WolfLive.Api) is one of unofficial WOLF libraries, developed by Alec.

In this FAQ article, I explain some differences between **WolfLive.Api** and **Wolfringo**, to make Wolfringo easier to use for anyone who used WolfLive.Api in the past.

## Will Wolfringo work with same framework versions as WolfLive.Api?
Yes! Both ***Wolfringo*** and [WolfLive.Api](https://github.com/calico-crusade/WolfLive.Api) target [.NET Standard 2.0](https://docs.microsoft.com/en-gb/dotnet/standard/net-standard), so in theory they should support exactly same framework versions.

## Can I use Dependency Injection?
[WolfLive.Api](https://github.com/calico-crusade/WolfLive.Api) supports Dependency Injection using <xref:System.IServiceProvider>. This allows sharing dependencies and services in a testable manner.

***Wolfringo*** does the same. In fact, **Wolfringo** is designed with Dependency Injection in mind, especially its Commands System!  
Feel free to check [Dependency Injection guide](xref:Guides.Commands.DependencyInjection) for more information.

## Do I need to manually register Wolfringo Commands Handler?
[WolfLive.Api](https://github.com/calico-crusade/WolfLive.Api) requires [command classes](https://github.com/calico-crusade/WolfLive.Api/wiki/Commands#wolfliveapi-commands) to be manually registered in Program.cs.

***Wolfringo*** doesn't require you to register them manually if they are marked with [\[CommandsHandler\] attribute](xref:TehGM.Wolfringo.Commands.CommandsHandlerAttribute). Manual registration is needed only if you don't add the attribute, or remove starting assembly from automatic loading. See [Enabling Commands guide](xref:Guides.Commands.Intro) for more information.

## Does Wolfringo Commands Handler need to inherit from any class?
[WolfLive.Api](https://github.com/calico-crusade/WolfLive.Api) requires [command classes](https://github.com/calico-crusade/WolfLive.Api/wiki/Commands#wolfliveapi-commands) to inherit from `WolfContext`. This is not an uncommon design - I saw that in various Discord libraries as well.

I found that a bit limiting, since C# only allows inheriting from one class. Instead, I chose to mark Wolfringo [Commands Handlers](xref:Guides.Commands.Handlers#handlers) with an attribute.  
Because of this, your class won't have any command-related properties out of the box. But don't worry - you can access a @TehGM.Wolfringo.Commands.CommandContext easily by using [Command Parameters](xref:Guides.Commands.Handlers#commandcontext).

## Do Wolfringo Commands allow requiring permission, or work in group only?
[WolfLive.Api](https://github.com/calico-crusade/WolfLive.Api) allows restricting commands execution using attributes (called [Filters](https://github.com/calico-crusade/WolfLive.Api/wiki/Commands#filters)). **WolfLive.Api** also allows you to create your own filters.

***Wolfringo*** is no different in that regard! Feel free to check [Commands Requirements guide](xref:Guides.Commands.Requirements) for more details.

## Can I do something on certain non-message events?
[WolfLive.Api](https://github.com/calico-crusade/WolfLive.Api) allows you to design [EventTemplates](https://github.com/calico-crusade/WolfLive.Api/wiki/Events) and add them as a listener to a string-identified event.

***Wolfringo*** allows you to do that as well, and works pretty similar - but you do not need to create your own event template. Wolfringo's @TehGM.Wolfringo.IWolfClient has a special [AddMessageListener&lt;T&gt;(delegate)](xref:TehGM.Wolfringo.WolfClientExtensions.AddMessageListener``1(TehGM.Wolfringo.IWolfClient,System.Action{``0})) method, where T is any <xref:TehGM.Wolfringo.IWolfMessage>. Using this method, you can add event listener to any message type, and it'll be triggered whenever a message of that type is received by the client.

```csharp
_client.AddMessageListener<GroupUpdateEvent>(OnGroupUpdated);

private async void OnGroupUpdated(GroupUpdateEvent message)
{
   // do something here
}
```