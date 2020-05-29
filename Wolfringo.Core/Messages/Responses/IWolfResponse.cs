using System.Net;

namespace TehGM.Wolfringo.Messages.Responses
{
    public interface IWolfResponse
    {
        HttpStatusCode StatusCode { get; }
    }
}
