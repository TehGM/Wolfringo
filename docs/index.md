# Wolfringo
Wolfringo is a .NET library for [WOLF](https://wolf.live) (previously [Palringo](https://palringo.com)).

This library is designed with extensibility through Dependency Injection in mind, and is compatible with ASP.NET Core and other .NET Core Hosting scenarios through Wolfringo.Hosting package.

Library works with strongly-typed messages and responses, that are serialized when sending and deserialized when receiving. Message listeners can be invoked by message type, giving full benefit of strong typing. Additionally, Wolfringo.Utilities package provides a Sender extensions class, which abstracts common sending tasks. Utilities package is included by default with Wolfringo meta-package.

Wolfringo provides a built in Commands System. Commands System uses attributes to mark commands, which greatly reduces amount of boilerplace code needed.
The Commands System follows the design principles of entire Wolfringo library, and therefore is easily extensible and easily customizable thanks to Dependency Injection.

## Contents
### This documentation page
Check [Documentation](/guides) for guides on how to use Wolfringo library.

Check [API Reference](/api) to check documentation on all Wolfringo types and methods.

Check [Changelog](/changelog) to view Wolfringo changelog.

### Examples
Check [Example projects](https://github.com/TehGM/Wolfringo/Examples) for basic examples on getting started.

Check [Pic Size Bot by TehGM](https://github.com/TehGM/WolfBot-Size) for live example.