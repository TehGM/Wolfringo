namespace TehGM.Wolfringo.Utilities.Internal
{
    /// <summary>Caches Wolf entities.</summary>
    /// <typeparam name="TEntity">Type of cached entity.</typeparam>
    public interface IWolfEntityCache<TEntity> where TEntity : IWolfEntity
    {
        /// <summary>Retrieves cached entity.</summary>
        /// <param name="id">ID of entity to retrieve.</param>
        /// <returns>Retrieved entity if found; otherwise null (or default).</returns>
        TEntity Get(uint id);
        /// <summary>Stores entity in cache, replacing any entity with the same ID.</summary>
        /// <param name="item">Entity to store.</param>
        void AddOrReplace(TEntity item);
        /// <summary>Removes entity from cache.</summary>
        /// <param name="id">ID of entity to remove.</param>
        void Remove(uint id);
        /// <summary>Removes all entites from the cache.</summary>
        void Clear();
    }

    /// <summary>Caches Wolf entities, additionally grouping them with a key.</summary>
    /// <typeparam name="TKey">Type of the grouping key.</typeparam>
    /// <typeparam name="TEntity">Type of cached entity.</typeparam>
    public interface IWolfEntityCache<TKey, TEntity> where TEntity : IWolfEntity
    {
        /// <summary>Retrieves cached entity.</summary>
        /// <param name="key">Key of the group for the entity.</param>
        /// <param name="id">ID of entity to retrieve.</param>
        /// <returns>Retrieved entity if found; otherwise null (or default).</returns>
        TEntity Get(TKey key, uint id);
        /// <summary>Stores entity in cache, replacing any entity with the same ID and Key.</summary>
        /// <param name="key">Key of the group for the entity.</param>
        /// <param name="item">Entity to store.</param>
        void AddOrReplace(TKey key, TEntity item);
        /// <summary>Removes entity from cache.</summary>
        /// <param name="key">Key of the group for the entity.</param>
        /// <param name="id">ID of entity to remove.</param>
        void Remove(TKey key, uint id);
        /// <summary>Removes all entities grouped with provided key.</summary>
        /// <param name="key">Key of the group for the entity.</param>
        void Clear(TKey key);
        /// <summary>Removes all entites from the cache.</summary>
        void ClearAll();
    }
}
