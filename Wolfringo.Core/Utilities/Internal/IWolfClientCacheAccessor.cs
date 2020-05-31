namespace TehGM.Wolfringo.Utilities.Internal
{
    public interface IWolfClientCacheAccessor
    {
        WolfUser GetCachedUser(uint id);
        WolfGroup GetCachedGroup(uint id);
        WolfCharm GetCachedCharm(uint id);
    }
}
