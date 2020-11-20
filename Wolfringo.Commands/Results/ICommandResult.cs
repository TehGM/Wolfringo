using System;
using System.Collections.Generic;
using System.Text;

namespace TehGM.Wolfringo.Commands
{
    public interface ICommandResult
    {
        bool IsSuccess { get; }
    }
}
