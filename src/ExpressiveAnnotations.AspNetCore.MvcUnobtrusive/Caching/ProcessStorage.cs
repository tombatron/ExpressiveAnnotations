using System;
using System.Collections.Concurrent;
using System.Threading;

namespace ExpressiveAnnotations.AspNetCore.MvcUnobtrusive.Caching
{
    internal static class ProcessStorage<TKey, TValue>
    {
        private static readonly ConcurrentDictionary<TKey, Lazy<TValue>> _cache = new ConcurrentDictionary<TKey, Lazy<TValue>>();

        public static TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
        {
            var lazyResult = _cache.GetOrAdd(key, k => new Lazy<TValue>(() => valueFactory(k), LazyThreadSafetyMode.ExecutionAndPublication));

            return lazyResult.Value;
        }

        public static void Clear() => _cache.Clear();
    }
}