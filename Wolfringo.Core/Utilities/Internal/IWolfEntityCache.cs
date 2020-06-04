namespace TehGM.Wolfringo.Utilities.Internal
{
    public interface IWolfEntityCache<TEntity> where TEntity : IWolfEntity
    {
        TEntity Get(uint id);
        void AddOrReplace(TEntity item);
        bool Remove(uint id);
        void Clear();
    }

    public interface IWolfEntityCache<TKey, TEntity> where TEntity : IWolfEntity
    {
        TEntity Get(TKey key, uint id);
        void AddOrReplace(TKey key, TEntity item);
        bool Remove(TKey key, uint id);
        void Clear(TKey key);
        void ClearAll();
    }
}
