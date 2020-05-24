using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace TehGM.Wolfringo.Messages.Responses
{
    public interface IWolfResponse
    {
        HttpStatusCode ResponseCode { get; }
    }
}
