---
uid: Guides.Features.Avatars
---

# Working with Avatars
Avatars of users and groups are one of core features of WOLF. For this reason you might want to work with them. In this guide, I briefly explain what Wolfringo provides in that matter.

> [!CAUTION]
> As of Wolfringo v2.1.1, @TehGM.Wolfringo.Utilities.AvatarUtilities class has been obsoleted, due to old links format no longer working and new seemingly requiring API calls. This means this class will need to get reworked.
> Please avoid using this class until it's been redesigned and fixed in a future update.

## Retrieving avatars
Since [version 1.1.0](https://github.com/TehGM/Wolfringo/releases/tag/1.0.0), [Wolfringo.Utilities](https://www.nuget.org/packages/Wolfringo.Utilities) (installed automatically by [Wolfringo metapackage](https://www.nuget.org/packages/Wolfringo)) includes a set of small extension methods contained in @TehGM.Wolfringo.Utilities.AvatarUtilities class. These methods are designed to help with retrieving user avatar.

#### Getting URL
WOLF exposes avatar images through URL. @TehGM.Wolfringo.Utilities.AvatarUtilities has a few methods that will help you format this link.

```csharp
// AvatarUtilities is included in Utilities namespace
using TehGM.Wolfringo.Utilities;

// you can get URL to user or group avatars
WolfUser user = await _client.GetUserAsync(2644384);
WolfGroup group = await _client.GetGroupAsync("wolf");
string userAvatarURL = user.GetAvatarURL();
string groupAvatarURL = group.GetAvatarURL();
```

#### Downloading Avatar
The recommended way to download an avatar is to use a @System.Net.Http.HttpClient to download image from URL. It is also recommended that you either cache your @System.Net.Http.HttpClient or use @System.Net.Http.IHttpClientFactory to avoid issues explained by Microsoft in [Use IHttpClientFactory to implement resilient HTTP requests](https://docs.microsoft.com/en-gb/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests#issues-with-the-original-httpclient-class-available-in-net-core) article.

@TehGM.Wolfringo.Utilities.AvatarUtilities also includes special extension method `DownloadAvatarAsync`. It'll do basic download of the avatar for you, but it is limited in functionality. This method automatically builds the URL and downloads the avatar bytes.  
If avatar couldn't be found (for example, when the user/group does not have a custom avatar set), this method will return `null`. When any other error occurs when downloading the avatar, @System.Net.Http.HttpRequestException will be thrown.

```csharp
// AvatarUtilities is included in Utilities namespace
using TehGM.Wolfringo.Utilities;

// you can get URL to user or group avatars
WolfUser user = await _client.GetUserAsync(2644384);
WolfGroup group = await _client.GetGroupAsync("wolf");
byte[] userAvatar = await user.DownloadAvatarAsync();
byte[] groupAvatar = await group.DownloadAvatarAsync();
```

Once the avatar bytes are downloaded, you can send it using [Sender Utility](xref:Guides.Features.Sender), save to a file, modify, or anything you want to do!

##### Providing a HttpClient
`DownloadAvatarAsync` has an overload that takes a @System.Net.Http.HttpClient as one of parameters. This allows you to cache your client, or use @System.Net.Http.IHttpClientFactory. This means you can also set your headers, such as `User-Agent`.  

> [!WARNING]  
> Note: when you provide your own @System.Net.Http.HttpClient, you need to dispose it yourself, as @TehGM.Wolfringo.Utilities.AvatarUtilities will not dispose it automatically to allow caching, re-use etc.

```csharp
// AvatarUtilities is included in Utilities namespace
using TehGM.Wolfringo.Utilities;

// download user avatar
using (HttpClient myHttpClient = new HttpClient())
{
    myHttpClient.DefaultRequestHeaders.Add("User-Agent", "MyBot v1.0.0");
    byte[] userAvatar = await user.DownloadAvatarAsync(myHttpClient);
}
```

## Changing Avatar
Currently, you cannot change your bot's Avatar using Wolfringo. This is because WOLF protocol v3 does not seem to allow doing that at this time.

Once WOLF protocol supports avatar changing, this feature will be added in one of Wolfringo updates.