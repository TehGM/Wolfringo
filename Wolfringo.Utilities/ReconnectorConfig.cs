using System;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace TehGM.Wolfringo.Utilities
{
    /// <summary>Represents configuration for <see cref="WolfClientReconnector"/>.</summary>
    public class ReconnectorConfig
    {
        /// <summary>Cancellation token for cancelling reconnection.</summary>
        public CancellationToken CancellationToken { get; set; }
        /// <summary>Delay between reconnection attempts.</summary>
        public TimeSpan ReconnectionDelay { get; set; }
        /// <summary>Max reconnection attempts.</summary>
        /// <remarks>Value of 0 means reconnection will not be attempted. Negative values will be treated as infinite.
        /// <para>Defaults to 5 times.</para></remarks>
        public int ReconnectAttempts { get; set; }
        /// <summary>Logger to log messages to.</summary>
        public ILogger Log { get; set; }

        public ReconnectorConfig(int attempts, TimeSpan delay, ILogger logger = null, CancellationToken cancellationToken = default)
        {
            if (attempts == 0)
                throw new ArgumentException("Reconnection attempts cannot equal 0", nameof(attempts));
            this.CancellationToken = cancellationToken;
            this.Log = logger;
            this.ReconnectAttempts = attempts;
            this.ReconnectionDelay = delay;
        }

        /// <summary>Default configuration, attempting to reconnect at 500ms interval, max 5 times, and without cancellation and logging support.</summary>
        public ReconnectorConfig()
            : this(5, TimeSpan.FromSeconds(0.5), null, default) { }
    }
}
