# FAQ
## Does Wolfringo support translations?
Short answer: no.

Long answer: I believe that translating text isn't in scope of a bot library.  
If you want to use translations in your bot, you can make a separate standalone service class. Commands System in Wolfringo supports dependency injection, so your translator can easily be injected to any handler.