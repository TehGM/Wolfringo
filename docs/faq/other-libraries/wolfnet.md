---
uid: FAQ.OtherLibs.WolfNet
---

# FAQ - WOLF.Net and Wolfringo
[WOLF.Net](https://github.com/dawalters1/Wolf.Net) is one of the more commonly used unofficial WOLF libraries, developed by Dave.

In this FAQ article, I explain some differences between **WOLF.Net** and **Wolfringo**, to make Wolfringo easier to use for anyone who used WOLF.Net in the past.

## Will Wolfringo work with same framework versions as WOLF.Net?
###### Short answer
Yes!
###### Long answer
[WOLF.Net](https://github.com/dawalters1/Wolf.Net) requires [.NET Core 3.0](https://dotnet.microsoft.com/download/dotnet-core/3.0) or above (including support for [.NET 5](https://dotnet.microsoft.com/download/dotnet/5.0)).

***Wolfringo*** is targetting [.NET Standard 2.0](https://docs.microsoft.com/en-gb/dotnet/standard/net-standard) instead. That means that it not only supports the same versions as **WOLF.Net** - it also supports .NET Framework 4.6.1 and [.NET Core 2.0](https://dotnet.microsoft.com/download/dotnet-core/2.0).

## Does Wolfringo have Forms?
[WOLF.Net](https://github.com/dawalters1/Wolf.Net) features Forms - a way to have a set of ordered prompts to the user in a group. This is useful when for example creating an item that requires a set of values that can be varied, and therefore tough to determine using command arguments.

***Wolfringo*** allows doing something similar with a feature called [Interactive](xref:Guides.Features.Interactive). It takes a slightly different approach - instead od declaring a Form as a class that declares stages as methods, it awaits next message inline, making it more similar to functional programming.  
This means that with Wolfringo, any kind of command or event can start a "form", do it conditionally, and mix normal commands with "Form" commands.  
Wolfringo also allows setting fully [custom waiting conditions](xref:Guides.Features.Interactive#custom-criteria).

## Does Wolfringo support translations?
[WOLF.Net](https://github.com/dawalters1/Wolf.Net) allows setting a key string, and assign it a language-dependent translation. This allows making a command reply in a language the command was requested in.

***Wolfringo*** doesn't have such functionality. As I explained in [main FAQ](xref:FAQ), I think it's not really a responsibility of a bot library. Instead, Wolfringo [Commands System](xref:Guides.Commands.Intro) fully supports [Dependency Injection](xref:Guides.Commands.DependencyInjection).  
This means that when using Wolfringo, you can use any custom service or even a fully-fledged library designed to do translations. Commands themselves can also have multiple [alias-like entries](xref:Guides.Commands.Handlers#aliases), so you can put in multiple command attributes per method to achieve multiple language triggers.

## Do Wolfringo Commands allow requiring permission, or work in group only?
[WOLF.Net](https://github.com/dawalters1/Wolf.Net) allows restricting commands execution using attributes.

***Wolfringo*** is no different in that regard! Feel free to check [Commands Requirements guide](xref:Guides.Commands.Requirements) for more details.

## Can I do something on certain non-message events?
[WOLF.Net](https://github.com/dawalters1/Wolf.Net) has separate registrations for events (such as `GroupUpdated`) to let you handle them.

***Wolfringo*** allows you to do that as well - but in Wolfringo, it's done slightly differently. Wolfringo's @TehGM.Wolfringo.IWolfClient has a special [AddMessageListener&lt;T&gt;(delegate)](xref:TehGM.Wolfringo.WolfClientExtensions.AddMessageListener``1(TehGM.Wolfringo.IWolfClient,System.Action{``0})) method, where T is any <xref:TehGM.Wolfringo.IWolfMessage>. Using this method, you can add event listener to any message type, and it'll be triggered whenever a message of that type is received by the client.

```csharp
_client.AddMessageListener<GroupUpdateEvent>(OnGroupUpdated);

private async void OnGroupUpdated(GroupUpdateEvent message)
{
   // do something here
}
```