using System;
using System.Threading;
using System.Threading.Tasks;

namespace TehGM.Wolfringo.Commands
{
    /// <summary>Represents any special validation that message needs to pass for command to be executed.</summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public abstract class CommandRequirementAttribute : Attribute
    {
        public abstract Task<bool> RunAsync(ICommandContext context, CancellationToken cancellationToken = default);
    }
}
