namespace TehGM.Wolfringo.Utilities.Internal
{
    public static class WolfEntityCacheExtensions
    {
        public static bool TryGet<T>(this IWolfEntityCache<T> cache, uint id, out T value) where T : IWolfEntity
        {
            value = cache.Get(id);
            return value != null;
        }

        public static bool AddOrReplaceIfChanged<T>(this IWolfEntityCache<T> cache, T item) where T : IWolfEntity
        {
            T existingItem = cache.Get(item.ID);
            if (existingItem == null || !item.Equals(existingItem))
            {
                cache.AddOrReplace(item);
                return true;
            }
            return false;
        }
    }
}
