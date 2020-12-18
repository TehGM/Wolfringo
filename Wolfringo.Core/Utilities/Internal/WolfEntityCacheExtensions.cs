namespace TehGM.Wolfringo.Utilities.Internal
{
    /// <summary>Extensions for <see cref="IWolfEntityCache{TEntity}"/></summary>
    public static class WolfEntityCacheExtensions
    {
        /// <summary>Adds entity, or replaces it if the existing one changed.</summary>
        /// <remarks>In case entity with same ID already exists, this method will compare existing entity with it's Equals method.
        /// If Equals returns true, the entity will not be replaced.</remarks>
        /// <typeparam name="T">Type of cached entity.</typeparam>
        /// <param name="cache">Cache to add or replace item in.</param>
        /// <param name="item">Item to add or replace.</param>
        public static void AddOrReplaceIfChanged<T>(this IWolfEntityCache<T> cache, T item) where T : IWolfEntity
        {
            T existingItem = cache.Get(item.ID);
            if (existingItem == null || !item.Equals(existingItem))
                cache.AddOrReplace(item);
        }
    }
}
