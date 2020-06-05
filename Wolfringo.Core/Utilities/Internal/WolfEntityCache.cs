using System.Collections.Generic;

namespace TehGM.Wolfringo.Utilities.Internal
{
    /// <inheritdoc/>
    public class WolfEntityCache<TEntity> : IWolfEntityCache<TEntity> where TEntity : IWolfEntity
    {
        private readonly IDictionary<uint, TEntity> _items = new Dictionary<uint, TEntity>();

        /// <inheritdoc/>
        public void AddOrReplace(TEntity item)
            => _items[item.ID] = item;

        /// <inheritdoc/>
        public TEntity Get(uint id)
        {
            _items.TryGetValue(id, out TEntity result);
            return result;
        }

        /// <inheritdoc/>
        public void Remove(uint id)
            => _items.Remove(id);

        /// <inheritdoc/>
        public void Clear()
            => _items.Clear();
    }

    /// <inheritdoc/>
    public class WolfEntityCache<TKey, TEntity> : IWolfEntityCache<TKey, TEntity> where TEntity : IWolfEntity
    {
        private readonly IDictionary<TKey, IWolfEntityCache<TEntity>> _items = new Dictionary<TKey, IWolfEntityCache<TEntity>>();

        /// <inheritdoc/>
        public void AddOrReplace(TKey key, TEntity item)
        {
            if (!_items.TryGetValue(key, out IWolfEntityCache<TEntity> subCache))
            {
                subCache = new WolfEntityCache<TEntity>();
                _items.Add(key, subCache);
            }
            subCache.AddOrReplace(item);
        }

        /// <inheritdoc/>
        public void Clear(TKey key)
        {
            if (_items.TryGetValue(key, out IWolfEntityCache<TEntity> subCache))
                subCache?.Clear();
        }

        /// <inheritdoc/>
        public void ClearAll()
        {
            foreach (IWolfEntityCache<TEntity> subCache in _items.Values)
                subCache?.Clear();
        }

        /// <inheritdoc/>
        public TEntity Get(TKey key, uint id)
        {
            if (_items.TryGetValue(key, out IWolfEntityCache<TEntity> subCache) && subCache != null)
                return subCache.Get(id);
            return default;
        }

        /// <inheritdoc/>
        public void Remove(TKey key, uint id)
        {
            if (_items.TryGetValue(key, out IWolfEntityCache<TEntity> subCache) && subCache != null)
                subCache.Remove(id);
        }
    }
}
