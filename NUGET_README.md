
# Wolfringo
[![Nuget](https://img.shields.io/nuget/v/Wolfringo)](https://www.nuget.org/packages/Wolfringo/) [![GitHub top language](https://img.shields.io/github/languages/top/TehGM/Wolfringo)](https://github.com/TehGM/Wolfringo) [![GitHub](https://img.shields.io/github/license/TehGM/Wolfringo)](LICENSE) [![GitHub Workflow Status](https://img.shields.io/github/workflow/status/TehGM/Wolfringo/.NET%20Core%20Build)](https://github.com/TehGM/Wolfringo/actions) [![GitHub issues](https://img.shields.io/github/issues/TehGM/Wolfringo)](https://github.com/TehGM/Wolfringo/issues)

This is a .NET library for WOLF (previously Palringo).

This library is designed with extensibility through Dependency Injection in mind, and is compatible with ASP.NET Core and other .NET Core Hosting scenarios through [Wolfringo.Hosting](https://www.nuget.org/packages/Wolfringo.Hosting/) package.

Library works with strongly-typed messages and responses, that are serialized when sending and deserialized when receiving. Message listeners can be invoked by message type, giving full benefit of strong typing. Additionally, [Wolfringo.Utilities](https://www.nuget.org/packages/Wolfringo.Utilities/) package provides a Sender extensions class, which abstracts common sending tasks. Utilities package is included by default with [Wolfringo](https://www.nuget.org/packages/Wolfringo/) meta-package.

Wolfringo provides a built in Commands System. Commands System uses attributes to mark commands, which greatly reduces amount of boilerplace code needed.  
The Commands System follows the design principles of entire Wolfringo library, and therefore is easily extensible and easily customizable thanks to Dependency Injection.

### Documentation
A full [Documentation](https://wolfringo.tehgm.net) is now available - it includes Tutorials and Guides, as well as full API Reference.

### Source Repository
Wolfringo is open source, with code hosted on [GitHub](https://github.com/TehGM/Wolfringo).