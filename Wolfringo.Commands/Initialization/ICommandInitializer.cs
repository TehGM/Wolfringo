using System.Reflection;

namespace TehGM.Wolfringo.Commands.Initialization
{
    public interface ICommandInitializer
    {
        ICommandInstance InitializeCommand(CommandAttributeBase commandAttribute, MethodInfo method);
    }
}
