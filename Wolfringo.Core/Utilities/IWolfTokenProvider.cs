namespace TehGM.Wolfringo.Utilities
{
    /// <summary>Provides a token used by Wolf client when connecting.</summary>
    public interface IWolfTokenProvider
    {
        /// <summary>Get a WOLF token.</summary>
        /// <returns>WOLF token.</returns>
        string GetToken();
    }
}
