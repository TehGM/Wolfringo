---
uid: FAQ.OtherLibs.WolfApiTs
---

# FAQ - WOLFApi-TS and Wolfringo
[WOLFApi-TS](https://github.com/masamesa/WOLFApi-TS) is a TypeScript unofficial WOLF library, developed by Masa.

In this FAQ article, I explain some differences between **WOLFApi-TS** and **Wolfringo**, to make Wolfringo easier to use for anyone who used WOLFApi-TS in the past.

## Will Wolfringo work with same framework versions as WOLFApi-TS?
No. Not at all.

[WOLFApi-TS](https://github.com/masamesa/WOLFApi-TS) is designed for TypeScript, while **Wolfringo** is designed for .NET Languages (C#/VB.NET/etc).

## Does Wolfringo have Plugins?
[WOLFApi-TS](https://github.com/masamesa/WOLFApi-TS) calls them "Plugins", but functionally they are the same as "Commands" in other libraries.

***Wolfringo*** features a full [Commands System](xref:Guides.Commands.Intro).

## Do Wolfringo Commands allow requiring permission, or work in group only?
[WOLFApi-TS](https://github.com/masamesa/WOLFApi-TS) allows restricting commands execution with a `iPluginOptions` object.

***Wolfringo*** allows doing the same using [C# Attributes](https://docs.microsoft.com/en-gb/dotnet/csharp/programming-guide/concepts/attributes/). Feel free to check [Commands Requirements guide](xref:Guides.Commands.Requirements) for more details.

## Does Wolfringo have a class that allows me to easily reply to a message?
[WOLFApi-TS](https://github.com/masamesa/WOLFApi-TS)'s WolfClient has a `Messaging` property that allows you to reply to the message.

***Wolfringo*** includes a similar feature, but instead of a property, it has a set of extension methods for <xref:TehGM.Wolfringo.IWolfClient>. They'll be available in any of your client instances as long as you have [Wolfringo.Utilities](https://www.nuget.org/packages/Wolfringo.Utilities) installed (installed automatically with main [Wolfringo](https://www.nuget.org/packages/Wolfringo) metapackage).

```csharp
private async void OnChatMessage(ChatMessage message)
{
    await _client.ReplyTextAsync(message, "Okay, will do!");
}
```
Check [Sender Utility guide](xref:Guides.Features.Sender#sending-messages) for more details.