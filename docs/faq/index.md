---
uid: FAQ
---

# FAQ
## Does Wolfringo support translations?
##### Short answer
No.
##### Long answer
I believe a bot library is not supposed to deal with translations. If it's well-planned, it should focus on server connection, messages, etc. If a bot library tries to also support translations, it clearly tries to do things it should not be concerned with. It's simply out of scope.

If you want to use translations in your bot, you can make a separate standalone service class. Commands System in Wolfringo supports dependency injection, so your translator can easily be injected to any handler.  
You most likely can also find other C# libraries designed specifically for this purpose. They should work with Wolfringo just fine.

## Why group member list is sometimes empty?
This is a known issue, and since it's a bug within WOLF protocol, unfortunately there isn't much to do besides requesting group members again. This can be done by requesting group using Sender Utility, which checks group members list and re-requests it internally. It can also be done by sending @TehGM.Wolfringo.Messages.GroupMembersListMessage.