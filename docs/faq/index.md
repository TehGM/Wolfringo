---
uid: FAQ
---

# FAQ
## Does Wolfringo support translations?
###### Short answer
No.
###### Long answer
I believe a bot library is not supposed to deal with translations. If it's well-planned, it should focus on server connection, messages, etc. If a bot library tries to also support translations, it clearly tries to do things that it should not be concerned with. It's simply out of scope.

If you want to use translations in your bot, you can make a separate standalone service class. [Commands System](xref:Guides.Commands.Intro) in Wolfringo supports [dependency injection](xref:Guides.Commands.DependencyInjection), so your translator can easily be injected into any handler.  
You most likely can also find other C# libraries designed specifically for this purpose. They should work with Wolfringo just fine.

## Can I use different caching solution, Redis for example?
Since Wolfringo v2.0, you can implement your own <xref:TehGM.Wolfringo.Caching.IWolfClientCache>, which means you can use anything you desire for caching, including Redis, local files - virtually anything!

See [Customizing Client Cache](xref:Guides.Customizing.Client.ClientCache#custom-client-cache) guide for more details!

## Why group member list is sometimes empty?
This is a known issue, and since it's a bug within WOLF protocol, unfortunately there isn't much to do besides requesting group members again. This can be done by requesting group using [Sender Utility](xref:Guides.Features.Sender#wolfgroup), which checks group members list and re-requests it internally. It can also be done by sending @TehGM.Wolfringo.Messages.GroupMembersListMessage.

## I am used to X library for WOLF - how can I do Y in Wolfringo?
Good news - I have created a small set of FAQs for popular WOLF libraries to help you answer this question! Check [Other Libraries](xref:FAQ.OtherLibs) FAQ section to see the list!