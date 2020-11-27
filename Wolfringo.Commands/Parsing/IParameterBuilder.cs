using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using TehGM.Wolfringo.Commands.Results;

namespace TehGM.Wolfringo.Commands.Parsing
{
    /// <summary>Utility for building parameters when invoking a command method.</summary>
    public interface IParameterBuilder
    {
        /// <summary>Builds parameters.</summary>
        /// <param name="parameters">Collection of parameters to be built.</param>
        /// <param name="values">Values to use when building params.</param>
        /// <param name="cancellationToken">Cancellation token used to cancel building</param>
        /// <returns>Result of the parameters building.</returns>
        Task<ParameterBuildingResult> BuildParamsAsync(IEnumerable<ParameterInfo> parameters, ParameterBuilderValues values, CancellationToken cancellationToken = default);
    }
}
