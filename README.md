
# Wolfringo
[![Nuget](https://img.shields.io/nuget/v/Wolfringo)](https://www.nuget.org/packages/Wolfringo/) [![GitHub top language](https://img.shields.io/github/languages/top/TehGM/Wolfringo)](https://github.com/TehGM/Wolfringo) [![GitHub](https://img.shields.io/github/license/TehGM/Wolfringo)](LICENSE) [![GitHub Workflow Status](https://img.shields.io/github/workflow/status/TehGM/Wolfringo/.NET%20Core%20Build)](https://github.com/TehGM/Wolfringo/actions) [![GitHub issues](https://img.shields.io/github/issues/TehGM/Wolfringo)](https://github.com/TehGM/Wolfringo/issues)

This is a .NET library for WOLF (previously Palringo).

This library is designed with extensibility through Dependency Injection in mind, and is compatible with ASP.NET Core and other .NET Core Hosting scenarios through [Wolfringo.Hosting](https://www.nuget.org/packages/Wolfringo.Hosting/) package.

Library works with strongly-typed messages and responses, that are serialized when sending and deserialized when receiving. Message listeners can be invoked by message type, giving full benefit of strong typing. Additionally, [Wolfringo.Utilities](https://www.nuget.org/packages/Wolfringo.Utilities/) package provides a Sender extensions class, which abstracts common sending tasks. Utilities package is included by default with [Wolfringo](https://www.nuget.org/packages/Wolfringo/) meta-package.

Wolfringo provides a built in Commands System. Commands System uses attributes to mark commands, which greatly reduces amount of boilerplace code needed.  
The Commands System follows the design principles of entire Wolfringo library, and therefore is easily extensible and easily customizable thanks to Dependency Injection.

### Documentation
A full [Documentation](https://wolfringo.tehgm.net) is now available - it includes Tutorials and Guides, as well as full API Reference.

### Download
Most recent versions of this package are downloadable via [nuget.org](https://www.nuget.org/packages/Wolfringo/)!

1. Install package in your project
    ```cli
    Install-Package Wolfringo
    ```
2. *(.NET Core Host/ASP.NET Core only)* Install Wolfringo.Hosting package
    ```cli
    Install-Package Wolfringo.Hosting
    ```

#### Older versions
Older versions are available through GitHub Packages. See [Installation guide in Documentation](https://wolfringo.tehgm.net/guides/getting-started/installation#older-versions) for steps to install version before v0.4.0.

### Requirements
The library targets [.NET Standard 2.0](https://docs.microsoft.com/en-gb/dotnet/standard/net-standard), and therefore works with .NET Core 2.0+, .NET Framework 4.6.1+ and .NET 5+.

See [Introduction](https://wolfringo.tehgm.net/guides/index.html#requirements) in documentation for more details.

## Further development
#### Known bugs and missing features
- Avatar setting is not supported, due to Wolf protocol not supporting it yet.
- Spam filter settings is not supported.

### Contributing
To ask questions or give ideas for Wolfringo, start a new [Discussion](https://github.com/TehGM/Wolfringo/discussions).

In case you want to report a bug or request a new feature, open a new [Issue](https://github.com/TehGM/Wolfringo/issues).

If you want to contribute a patch or update, fork repository, implement the change, and open a pull request.

## License
Copyright (c) 2020 TehGM 

Licensed under [MIT License](LICENSE).
