---
uid: Guides.GettingStarted.Installation
---

# Installation Guide
Default means to install Wolfringo is through NuGet package.

[Wolfringo](https://www.nuget.org/packages/Wolfringo) metapackage will automatically install [Wolfringo.Core](https://www.nuget.org/packages/Wolfringo.Core), [Wolfringo.Commands](https://www.nuget.org/packages/Wolfringo.Commands), [Wolfringo.Utilities](https://www.nuget.org/packages/Wolfringo.Utilities) and [Wolfringo.Utilities.Interactive](https://www.nuget.org/packages/Wolfringo.Utilities.Interactive).  

### [Visual Studio Package Manager](#tab/install-with-vs)
1. Manage NuGet Packages for your bot project.  
![](/_images/guides/install-vs-1.png)
2. Click "Browse" and type "Wolfringo". Note: checking "Include prerelease" allows downloading beta versions.
3. Select "Wolfringo" package.  
![](/_images/guides/install-vs-2.png)
4. Press install in the window on the right.  
![](/_images/guides/install-vs-3.png)

### [Package Manager Console](#tab/install-with-pmconsole)
Run following command in VS Package Manager Console:  
```bash
Install-Package Wolfringo -Version 1.1.1
```

### [Command Line](#tab/install-with-cli)
Run following commands in command line:  
```bash
dotnet add package Wolfringo --version 1.1.1
dotnet restore
```

### [.csproj File](#tab/install-with-csproj)
Add following package reference to your .csproj file:  
```xml
<PackageReference Include="Wolfringo" Version="1.1.1" />
```
***

If you need more fine-grained control, you can install these packages individually instead of installing [Wolfringo](https://www.nuget.org/packages/Wolfringo).

## Installing for .NET Generic Host/ASP.NET Core
Wolfringo includes support for [.NET Generic Host](https://docs.microsoft.com/en-gb/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-3.0) (for example with ASP.NET Core - but not only). To enable it, I recommend installing [Wolfringo.Hosting](https://www.nuget.org/packages/Wolfringo.Hosting).

[Wolfringo.Hosting](https://www.nuget.org/packages/Wolfringo.Hosting) includes extension methods for @Microsoft.Extensions.DependencyInjection.IServiceCollection that can be used when registering services (for example, in ASP.NET Core's `Startup.cs`). Additionally, this package includes wrappers @TehGM.Wolfringo.Hosting.HostedWolfClient and @TehGM.Wolfringo.Hosting.Commands.HostedCommandsService, both of which implement @Microsoft.Extensions.Hosting.IHostedService to integrate with host lifetime.

### [Visual Studio Package Manager](#tab/install-with-vs)
1. Follow steps to [Install Wolfringo](#installation-guide).
2. Additionally install Wolfringo.Hosting package.  
![](/_images/guides/install-vs-4.png)

### [Package Manager Console](#tab/install-with-pmconsole)
1. Follow steps to [Install Wolfringo](#installation-guide).
2. Run following command in VS Package Manager Console:  
    ```bash
    Install-Package Wolfringo.Hosting -Version 1.1.1
    ```

### [Command Line](#tab/install-with-cli)
1. Follow steps to [Install Wolfringo](#installation-guide).
2. Run following commands in command line:  
    ```bash
    dotnet add package Wolfringo.Hosting --version 1.1.1
    dotnet restore
    ```

### [.csproj File](#tab/install-with-csproj)
1. Follow steps to [Install Wolfringo](#installation-guide).
2. Add following package reference to your .csproj file:  
    ```xml
    <PackageReference Include="Wolfringo.Hosting" Version="1.1.1" />
    ```
***

### Older versions
If you need to install older version ([0.1.0](https://github.com/TehGM/Wolfringo/releases/tag/0.1.0) - [0.3.4](https://github.com/TehGM/Wolfringo/releases/tag/0.3.4)) of Wolfringo, you need to use GitHub Packages. 

> Note: versions 0.1.X and 0.2.X are unstable and therefore deprecated. Use at own risk.  
> The first version that can be considered safe for use is [0.3.1](https://github.com/TehGM/Wolfringo/releases/tag/0.3.1).

### [Visual Studio Package Manager](#tab/install-with-vs)
1. Create a GitHub personal access token (PAT): https://github.com/settings/tokens/new. Make sure you check `read:packages` scope.
2. Manage NuGet Packages for your bot project.  
![](/_images/guides/install-vs-1.png)
3. Click "Settings" cog in the top right section of Package Manager window.  
![](/_images/guides/install-vs-5.png)
4. In the window that opens, press add button.  
![](/_images/guides/install-vs-6.png)
5. Once new item appears on the list, on the bottom of the window set Name to "TehGM's GitHub" and Source to "https://nuget.pkg.github.com/TehGM/index.json", then press "Update", and then "OK".  
![](/_images/guides/install-vs-7.png)
6. Select "TehGM's GitHub" package source.  
![](/_images/guides/install-vs-8.png)
7. A window will pop up with prompt for credentials.  
   For User name, input your GitHub Username.  
   For Password, input you GitHub PAT created in step 1.  
   Optionally, check box to remember credentials.  
   Press OK.
8. Select and install any of the Wolfringo Packages.  
![](/_images/guides/install-vs-9.png)


### [Package Manager Console](#tab/install-with-pmconsole)
1. Create a GitHub personal access token (PAT): https://github.com/settings/tokens/new. Make sure you check `read:packages` scope.
2. Run following commands to authenticate with GitHub Packages, replacing <GithubUsername> and <GithubToken> with your github username and generated PAT, respectively:  
    ```bash
    dotnet nuget add source https://nuget.pkg.github.com/TehGM/index.json -n "TehGM's GitHub" -u <GithubUsername> -p <GithubToken>
    ```
3. Install Wolfringo package in your project:  
    ```bash
    Install-Package Wolfringo -Source "TehGM's GitHub"
    ```
4. *(.NET Generic Host/ASP.NET Core only)* Install Wolfringo.Hosting package:  
    ```bash
    Install-Package Wolfringo.Hosting -Source "TehGM's GitHub"
    ```

    
### [Command Line](#tab/install-with-cli)
1. Create a GitHub personal access token (PAT): https://github.com/settings/tokens/new. Make sure you check `read:packages` scope.
2. Run following commands to authenticate with GitHub Packages, replacing <GithubUsername> and <GithubToken> with your github username and generated PAT, respectively:  
    ```bash
    dotnet nuget add source https://nuget.pkg.github.com/TehGM/index.json -n "TehGM's GitHub" -u <GithubUsername> -p <GithubToken>
    ```
3. Install package in your project:  
    ```bash
    dotnet add package Wolfringo --source "TehGM's GitHub"
    ```
4. *(.NET Generic Host/ASP.NET Core only)* Install Wolfringo.Hosting package:  
    ```bash
    dotnet add package Wolfringo.Hosting --source "TehGM's GitHub"
    ```
5. Restore packages:  
    ```bash
    dotnet restore
    ```

### [.csproj File](#tab/install-with-csproj)
1. Create a GitHub personal access token (PAT): https://github.com/settings/tokens/new. Make sure you check `read:packages` scope.
2. Run following commands to authenticate with GitHub Packages, replacing <GithubUsername> and <GithubToken> with your github username and generated PAT, respectively:
    ```bash
    dotnet nuget add source https://nuget.pkg.github.com/TehGM/index.json -n "TehGM's GitHub" -u <GithubUsername> -p <GithubToken>
    ```
3. Add Wolfringo package reference to your .csproj file:
    ```xml
    <PackageReference Include="Wolfringo" Version="0.3.4" />
    ```
4. *(.NET Generic Host/ASP.NET Core only)* Add Wolfringo.Hosting package reference to your .csproj file:
    ```xml
    <PackageReference Include="Wolfringo.Hosting" Version="0.3.3" />
    ```
5. Restore packages with command line:
    ```bash
    dotnet restore
    ```
***

See [GitHub Packages](https://help.github.com/en/packages/using-github-packages-with-your-projects-ecosystem/configuring-dotnet-cli-for-use-with-github-packages#installing-a-package) for more information about installing GitHub packages.