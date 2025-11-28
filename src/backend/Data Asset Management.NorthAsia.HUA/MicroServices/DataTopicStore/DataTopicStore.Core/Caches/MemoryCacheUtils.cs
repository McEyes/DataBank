using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace DataTopicStore.Core.Caches
{
    public static class MemoryCacheUtils
    {
        private static readonly MemoryCache _cache = new(new MemoryCacheOptions());

        public static TValue Get<TValue>(string key, TimeSpan timeSpan, Func<TValue> createValue) => Get(key, timeSpan, createValue, t => false);
        public static TValue Get<TValue>(string key, TimeSpan timeSpan, Func<TValue> createValue, Func<TValue, bool> noCreate)
        {
            return _cache.GetOrCreate(key, entry =>
            {
                TValue value = createValue();
                if (noCreate(value))
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(1);
                }
                else
                {
                    entry.AbsoluteExpirationRelativeToNow = timeSpan;
                }

                return value;
            });
        }

        public static bool Contains(string key) => _cache.TryGetValue(key, out _);
        public static void Set<TValue>(string key, TValue value, TimeSpan timeSpan) => _cache.Set(key, value, timeSpan);
        public static TValue Get<TValue>(string key) => _cache.Get<TValue>(key);
        public static void Remove(string key) => _cache.Remove(key);
        public static object Get(string key) => _cache.Get(key);
    }
}
