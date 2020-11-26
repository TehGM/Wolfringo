using System;
using System.Collections.Generic;
using System.Linq;

namespace TehGM.Wolfringo.Commands.Results
{
    public struct ParameterBuildingResult : ICommandResult, IMessagesCommandResult
    {
        public bool IsSuccess { get; }
        public Exception Exception { get; }
        public object[] Values { get; }
        public IEnumerable<string> Messages { get; }

        public ParameterBuildingResult(bool isSuccess, object[] values, IEnumerable<string> messages, Exception exception)
        {
            this.IsSuccess = isSuccess;
            this.Values = values ?? Array.Empty<object>();
            this.Exception = exception;
            this.Messages = messages?.Where(text => !string.IsNullOrWhiteSpace(text)) ?? Enumerable.Empty<string>();
        }

        public static ParameterBuildingResult Success(object[] values, IEnumerable<string> messages = null)
            => new ParameterBuildingResult(true, values, messages, null);

        public static ParameterBuildingResult Failure(Exception exception, IEnumerable<string> messages = null)
            => new ParameterBuildingResult(false, null, messages, exception);
    }
}
