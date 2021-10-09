using System;
using System.Collections.Generic;
using System.Linq;

namespace TehGM.Wolfringo.Caching.Internal
{
    /// <inheritdoc/>
    public class WolfCachedEntityCollection<TEntity> : IWolfCachedEntityCollection<TEntity> where TEntity : IWolfEntity
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
        public IEnumerable<TEntity> Find(Func<TEntity, bool> selector)
            => _items.Values.Where(selector);

        /// <inheritdoc/>
        public void Remove(uint id)
            => _items.Remove(id);

        /// <inheritdoc/>
        public void Clear()
            => _items.Clear();
    }

    /// <inheritdoc/>
    public class WolfCachedEntityCollection<TKey, TEntity> : IWolfCachedEntityCollection<TKey, TEntity> where TEntity : IWolfEntity
    {
        private readonly IDictionary<TKey, IWolfCachedEntityCollection<TEntity>> _items = new Dictionary<TKey, IWolfCachedEntityCollection<TEntity>>();

        /// <inheritdoc/>
        public void AddOrReplace(TKey key, TEntity item)
        {
            if (!_items.TryGetValue(key, out IWolfCachedEntityCollection<TEntity> subCache))
            {
                subCache = new WolfCachedEntityCollection<TEntity>();
                _items.Add(key, subCache);
            }
            subCache.AddOrReplace(item);
        }

        /// <inheritdoc/>
        public void Clear(TKey key)
        {
            if (_items.TryGetValue(key, out IWolfCachedEntityCollection<TEntity> subCache))
                subCache?.Clear();
        }

        /// <inheritdoc/>
        public void ClearAll()
        {
            foreach (IWolfCachedEntityCollection<TEntity> subCache in _items.Values)
                subCache?.Clear();
        }

        /// <inheritdoc/>
        public TEntity Get(TKey key, uint id)
        {
            if (_items.TryGetValue(key, out IWolfCachedEntityCollection<TEntity> subCache) && subCache != null)
                return subCache.Get(id);
            return default;
        }

        /// <inheritdoc/>
        public IEnumerable<TEntity> Find(TKey key, Func<TEntity, bool> selector)
        {
            if (_items.TryGetValue(key, out IWolfCachedEntityCollection<TEntity> subCache) && subCache != null)
                return subCache.Find(selector);
            return Enumerable.Empty<TEntity>();
        }

        /// <inheritdoc/>
        public void Remove(TKey key, uint id)
        {
            if (_items.TryGetValue(key, out IWolfCachedEntityCollection<TEntity> subCache) && subCache != null)
                subCache.Remove(id);
        }
    }
}
