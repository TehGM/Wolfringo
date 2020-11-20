using System;
using System.Threading;
using System.Threading.Tasks;

namespace TehGM.Wolfringo.Commands
{
    public interface ICommandInstance
    {
        Task<ICommandResult> CheckShouldRunAsync(ICommandContext context, CancellationToken cancellationToken = default);
        Task<ICommandResult> ExecuteAsync(ICommandContext context, IServiceProvider services, ICommandResult checkResult, CancellationToken cancellationToken = default);
    }
}
