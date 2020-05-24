namespace TehGM.Wolfringo.Messages.Responses
{
    public static class WolfResponseExtensions
    {
        public static bool IsSuccess(this IWolfResponse response)
        {
            int code = (int)response.ResponseCode;
            return code >= 200 && code <= 299;
        }

        public static bool IsError(this IWolfResponse response)
            => !response.IsSuccess();
    }
}
