# FAQ
## Does Wolfringo support translations?
Short answer: no.

Long answer: I believe that translating text isn't in scope of a bot library.  
If you want to use translations in your bot, you can make a separate standalone service class. Commands System in Wolfringo supports dependency injection, so your translator can easily be injected to any handler.

## Why group member list is empty sometimes?
This is a known issue, and since it's a bug within WOLF protocol, unfortunately there isn't much to do besides requesting group members again. This can be done by requesting group using Sender Utility, which checks group members list and re-requests it internally. It can also be done by sending @TehGM.Wolfringo.Messages.GroupMembersListMessage.