# Wolfringo
Wolfringo is a .NET library for [WOLF](https://wolf.live) (previously [Palringo](https://palringo.com)).

This library is designed with extensibility through Dependency Injection in mind, and is compatible with ASP.NET Core and other .NET Core Hosting scenarios through Wolfringo.Hosting package.

Library works with strongly-typed messages and responses, that are serialized when sending and deserialized when receiving. Message listeners can be invoked by message type, giving full benefit of strong typing. Additionally, Wolfringo.Utilities package provides a Sender extensions class, which abstracts common sending tasks. Utilities package is included by default with Wolfringo meta-package.

Wolfringo provides a built-in Commands System. Commands System uses attributes to mark commands, which greatly reduces amount of boilerplace code needed.
The Commands System follows the design principles of entire Wolfringo library, and therefore is easily extensible and easily customizable thanks to Dependency Injection.

## Contents
### This documentation page
Check [Documentation](xref:Guides.Introduction) for guides on how to use Wolfringo library.

Check [API Reference](xref:API) for documentation on all Wolfringo types and methods.

Check [FAQ](xref:FAQ) for frequently asked questions.

Check [Upgrading Guides](xref:Upgrading.Introduction) to see how to upgrade your bot to new major Wolfringo versions.

Check [Changelog](https://github.com/TehGM/Wolfringo/releases) to view Wolfringo changelog.

### Examples
Check [Example projects](https://github.com/TehGM/Wolfringo/tree/master/Examples) for basic examples on getting started.

Check [Pic Size Bot by TehGM](https://github.com/TehGM/WolfBot-Size) for live example (uses Wolfringo.Hosting).

### Source Code & Contributing
Source code is available on [GitHub](https://github.com/TehGM/Wolfringo).

To ask questions or share ideas, feel free to start a new [GitHub Discussion](https://github.com/TehGM/Wolfringo/discussions).

Any bugs? Requesting a specific feature? Please create a new [Issue on GitHub](https://github.com/TehGM/Wolfringo/issues).

## Sponsoring
If you want to sponsor my work, please check my page on [GitHub Sponsors](https://github.com/sponsors/TehGM)!

Alternatively, I also have [Patreon](https://patreon.com/TehGMdev) and [Buy Me a Coffee](https://www.buymeacoffee.com/TehGM) profiles.

## License
Copyright (c) 2020 TehGM

Wolfringo is licensed under [MIT License](https://github.com/TehGM/Wolfringo/blob/master/LICENSE).