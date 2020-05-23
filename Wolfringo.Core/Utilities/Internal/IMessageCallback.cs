using System.Reflection;

namespace TehGM.Wolfringo.Utilities.Internal
{
    public interface IMessageCallback
    {
        MethodInfo CallbackInfo { get; }
        bool TryInvoke(IWolfMessage message);
    }
}
