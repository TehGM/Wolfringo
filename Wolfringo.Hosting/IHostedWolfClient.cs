using Microsoft.Extensions.Hosting;

namespace TehGM.Wolfringo.Hosting
{
    /// <summary>A wrapper for <see cref="WolfClient"/> designed to use with .NET Core Host.</summary>
    /// <seealso cref="HostedWolfClient"/>
    /// <seealso cref="WolfClient"/>
    /// <seealso cref="IWolfClient"/>
    public interface IHostedWolfClient : IHostedService, IWolfClient { }
}
