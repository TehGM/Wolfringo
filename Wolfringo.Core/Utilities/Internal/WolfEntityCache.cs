using System.Collections.Generic;

namespace TehGM.Wolfringo.Utilities.Internal
{
    public class WolfEntityCache<T> : IWolfEntityCache<T> where T : IWolfEntity
    {
        private readonly IDictionary<uint, T> _items = new Dictionary<uint, T>();

        public void AddOrReplace(T item)
            => _items[item.ID] = item;

        public T Get(uint id)
        {
            _items.TryGetValue(id, out T result);
            return result;
        }

        public bool Remove(uint id)
            => _items.Remove(id);

        public void Clear()
            => _items.Clear();
    }
}
