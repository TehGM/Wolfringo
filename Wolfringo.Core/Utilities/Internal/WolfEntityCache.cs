using System.Collections.Generic;

namespace TehGM.Wolfringo.Utilities.Internal
{
    public class WolfEntityCache<TEntity> : IWolfEntityCache<TEntity> where TEntity : IWolfEntity
    {
        private readonly IDictionary<uint, TEntity> _items = new Dictionary<uint, TEntity>();

        public void AddOrReplace(TEntity item)
            => _items[item.ID] = item;

        public TEntity Get(uint id)
        {
            _items.TryGetValue(id, out TEntity result);
            return result;
        }

        public bool Remove(uint id)
            => _items.Remove(id);

        public void Clear()
            => _items.Clear();
    }

    public class WolfEntityCache<TKey, TEntity> : IWolfEntityCache<TKey, TEntity> where TEntity : IWolfEntity
    {
        private readonly IDictionary<TKey, IWolfEntityCache<TEntity>> _items = new Dictionary<TKey, IWolfEntityCache<TEntity>>();

        public void AddOrReplace(TKey key, TEntity item)
        {
            if (!_items.TryGetValue(key, out IWolfEntityCache<TEntity> subCache))
            {
                subCache = new WolfEntityCache<TEntity>();
                _items.Add(key, subCache);
            }
            subCache.AddOrReplace(item);
        }

        public void Clear(TKey key)
        {
            if (_items.TryGetValue(key, out IWolfEntityCache<TEntity> subCache))
                subCache?.Clear();
        }

        public void ClearAll()
        {
            foreach (IWolfEntityCache<TEntity> subCache in _items.Values)
                subCache?.Clear();
        }

        public TEntity Get(TKey key, uint id)
        {
            if (_items.TryGetValue(key, out IWolfEntityCache<TEntity> subCache) && subCache != null)
                return subCache.Get(id);
            return default;
        }

        public bool Remove(TKey key, uint id)
        {
            if (_items.TryGetValue(key, out IWolfEntityCache<TEntity> subCache) && subCache != null)
                return subCache.Remove(id);
            return false;
        }
    }
}
