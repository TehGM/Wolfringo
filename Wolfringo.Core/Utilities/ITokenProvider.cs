namespace TehGM.Wolfringo.Utilities
{
    /// <summary>Provides a token used by Wolf client when connecting.</summary>
    public interface ITokenProvider
    {
        /// <summary>Generate a new token.</summary>
        /// <param name="length">Expected length of the token.</param>
        /// <returns>Generated token.</returns>
        string GenerateToken(int length);
    }
}
