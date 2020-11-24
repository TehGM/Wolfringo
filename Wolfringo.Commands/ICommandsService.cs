using System.Threading;
using System.Threading.Tasks;
using TehGM.Wolfringo.Commands.Results;

namespace TehGM.Wolfringo.Commands
{
    public interface ICommandsService
    {
        Task StartAsync(CancellationToken cancellationToken = default);
        Task<ICommandResult> ExecuteAsync(ICommandContext context, CancellationToken cancellationToken = default);
    }
}
