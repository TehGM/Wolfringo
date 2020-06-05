namespace TehGM.Wolfringo.Messages.Responses
{
    public static class WolfResponseExtensions
    {
        /// <summary>Is response a success?</summary>
        /// <returns>True if response is a success; otherwise false.</returns>
        public static bool IsSuccess(this IWolfResponse response)
        {
            int code = (int)response.StatusCode;
            return code >= 200 && code <= 299;
        }

        /// <summary>Is response an error?</summary>
        /// <returns>True if response is an error; otherwise false.</returns>
        public static bool IsError(this IWolfResponse response)
            => !response.IsSuccess();
    }
}
