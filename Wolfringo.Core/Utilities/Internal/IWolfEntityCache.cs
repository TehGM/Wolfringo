namespace TehGM.Wolfringo.Utilities.Internal
{
    public interface IWolfEntityCache<T> where T : IWolfEntity
    {
        T Get(uint id);
        void AddOrReplace(T item);
        bool Remove(uint id);
        void Clear();
    }
}
